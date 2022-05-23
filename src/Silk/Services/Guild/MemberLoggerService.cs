using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;
using Remora.Results;
using Silk.Data.Entities;
using Silk.Data.MediatR.Users;
using Silk.Extensions;
using Silk.Extensions.Remora;
using Silk.Services.Data;
using Silk.Services.Interfaces;
using Silk.Shared.Constants;

namespace Silk.Services.Guild;

/// <summary>
/// A service for logging members joining and leaving.
/// </summary>
public class MemberLoggerService
{
    private const int JoinWarningThreshold = 3;
    private const int TwoWeeks             = 14;
    private const int TwoDays              = 2;
    private const int HalfDay              = 12;
    
    private readonly IMediator               _mediator;
    private readonly GuildConfigCacheService _configService;
    private readonly IChannelLoggingService   _channelLogger;
    
    public MemberLoggerService(IMediator mediator, GuildConfigCacheService configService, IChannelLoggingService channelLogger)
    {
        _mediator      = mediator;
        _configService = configService;
        _channelLogger = channelLogger;
    }

    public async Task<Result> LogMemberJoinAsync(Snowflake guildID, IGuildMember member)
    {
        if (!member.User.IsDefined(out var user))
            return Result.FromSuccess();
        
        var config = await _configService.GetModConfigAsync(guildID);
        
        if (!config.Logging.LogMemberJoins)
            return Result.FromSuccess();

        var channel = config.Logging.MemberJoins;
        
        if (channel is null)
            return Result.FromSuccess();

        var twoWeeksOld = user.ID.Timestamp.AddDays(TwoWeeks) > DateTimeOffset.UtcNow;
        var twoDaysOld = user.ID.Timestamp.AddDays(TwoDays) > DateTimeOffset.UtcNow;

        var userResult = await _mediator.Send(new GetOrCreateUser.Request(guildID, user.ID, member.JoinedAt));
        
        if (!userResult.IsDefined(out var userData))
            return Result.FromError(userResult.Error!);

        var sb = new StringBuilder();

        sb.AppendLine("Notes:");
        
        if (twoDaysOld)
            sb.AppendLine($"{Emojis.WarningEmoji} Account is only 2 days old");
        else if (twoWeeksOld)
            sb.AppendLine($"{Emojis.WarningEmoji} Account is only 2 weeks old");
        
        var userFields = new List<EmbedField>()
        {
            new("Username:", user.ToDiscordTag()),
            new("User ID:", user.ID.ToString()),
            new("User Created:", user.ID.Timestamp.ToTimestamp(TimestampFormat.LongDateTime)),
            new("User Joined:", userData.History.Last().JoinDate.ToTimestamp(TimestampFormat.LongDateTime) + '/' +
                                userData.History.Last().JoinDate.ToTimestamp())
        };
        
        if (userData.Infractions.Any())
        {
            sb.AppendLine($"{Emojis.WarningEmoji} User has infractions on record");
            userFields.Add
            (
             new
                 (
                  "Infractions:",
                  userData
                  .Infractions
                  .GroupBy(inf => inf.Type)
                  .Select(inf => $"{inf.Key}: {inf.Count()} time(s)")
                  .Join("\n"), true
                 )
            );
        }
        
        var userInfractionJoinBuffer = JoinWarningThreshold + userData
                                      .Infractions
                                      .Count
                                           (
                                            inf => 
                                                inf.Type is
                                                    InfractionType.Kick or
                                                    InfractionType.Ban or
                                                    InfractionType.SoftBan
                                           );
        
        if (userData.History.Count(g => g.GuildID == guildID) > userInfractionJoinBuffer)
            sb.AppendLine("Account has joined more than four times excluding infractions.");
        
        if (userData.History.Where(g => g.GuildID == guildID).Count(jd => jd.JoinDate.AddDays(TwoWeeks) > DateTimeOffset.UtcNow) > JoinWarningThreshold)
            sb.AppendLine("Account has joined more than three times in the last two weeks.");

        if (userData.History.Where(g => g.JoinDate.AddHours(HalfDay) > DateTimeOffset.UtcNow).DistinctBy(j => j.GuildID).Count() > JoinWarningThreshold)
            sb.AppendLine($"{Emojis.WarningEmoji} **Account has joined three or more servers in the last 12 hours**");
        
        var embed = new Embed()
        {
            Title       = "Member Joined",
            Description = sb.ToString(),
            Colour      = twoDaysOld ? Color.DarkRed : twoWeeksOld ? Color.Orange : Color.SeaGreen,
            Thumbnail   = new EmbedThumbnail(user.Avatar is null ? CDN.GetDefaultUserAvatarUrl(user).Entity.ToString() : CDN.GetUserAvatarUrl(user).Entity.ToString()),
            Fields      = userFields.ToArray()
        };
        
        return await _channelLogger.LogAsync(config.Logging.UseWebhookLogging, channel, null, embed);
    }
    
    public async Task<Result> LogMemberLeaveAsync(Snowflake guildID, IUser user)
    {
        var config = await _configService.GetModConfigAsync(guildID);
        
        if (!config.Logging.LogMemberLeaves)
            return Result.FromSuccess();

        var channel = config.Logging.MemberLeaves;
        
        if (channel is null)
            return Result.FromSuccess();

        var sb = new StringBuilder();
        
        var userResult = await _mediator.Send(new GetUser.Request(user.ID));
        
        var fields = new List<EmbedField>()
        {
            new("Username:", user.ToDiscordTag()),
            new("User ID:", user.ID.ToString()),
            new("User Created:", user.ID.Timestamp.ToTimestamp(TimestampFormat.LongDateTime))
        };
        
        if (userResult is null)
        {
            sb.AppendLine($"{Emojis.WarningEmoji} I don't have any prior data about this user, sorry!");
        }
        else
        {
            var lastJoin = userResult.History.Last();
            
            fields.Add(new("User Joined:", lastJoin.Joined.ToTimestamp(TimestampFormat.LongDateTime)));
            
            if (lastJoin.Joined + TimeSpan.FromHours(1) > DateTimeOffset.UtcNow)
                sb.AppendLine($"{Emojis.WarningEmoji} User joined less than an hour ago");
            
            else if (lastJoin.Joined + TimeSpan.FromDays(1) > DateTimeOffset.UtcNow)
                sb.AppendLine($"{Emojis.WarningEmoji} User joined less than a day ago");
        }

        var embed = new Embed()
        {
            Title       = "Member Left",
            Description = sb.ToString(),
            Colour      = Color.Firebrick,
            Thumbnail = new EmbedThumbnail(user.Avatar is null ? CDN.GetDefaultUserAvatarUrl(user).Entity.ToString() : CDN.GetUserAvatarUrl(user).Entity.ToString()),
            Fields = fields
        };
        
        return await _channelLogger.LogAsync(config.Logging.UseWebhookLogging, channel, null, embed);
    }
}