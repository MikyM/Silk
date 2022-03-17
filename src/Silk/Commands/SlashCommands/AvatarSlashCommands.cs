using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using OneOf;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Rest.Core;
using Remora.Results;
using Silk.Extensions.Remora;

namespace Silk.Commands.SlashCommands;

[SlashCommand]
public class AvatarSlashCommands : CommandGroup
{
    private readonly InteractionContext         _context;
    private readonly IDiscordRestGuildAPI       _guilds;
    private readonly IDiscordRestInteractionAPI _interactions;

    public AvatarSlashCommands(InteractionContext context, IDiscordRestGuildAPI guilds, IDiscordRestInteractionAPI interactions)
    {
        _context  = context;
        _guilds   = guilds;
        _interactions = interactions;
    }

    [Command("avatar")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Get the avatar of a user.")]
    public async Task<IResult> ShowAvatarAsync
    (
        [Description("The user's avatar you want to access, or leave blank to show your own!")] 
        IUser? user = null,
        
        [Description("Whether to pull the user's guild-specific avatar")]
        bool guild = false
    )
    {
        user ??= _context.User;

        IImageHash? hash = null;

        if (guild)
        {
            var memberResult = await _guilds.GetGuildMemberAsync(_context.GuildID.Value, user.ID);

            if (!memberResult.IsDefined(out var member))
                return await _interactions.EditOriginalInteractionResponseAsync
                    (
                     _context.ApplicationID,
                     _context.Token,
                     "I couldn't find that user in this guild!"
                    );
            
            if (!member.Avatar.IsDefined(out hash))
                return await _interactions.EditOriginalInteractionResponseAsync
                    (
                     _context.ApplicationID,
                     _context.Token,
                     "That user doesn't have a guild-specific avatar!"
                    );
        }
        
        var avatar = user.Avatar is null 
            ? CDN.GetDefaultUserAvatarUrl(user, imageSize: 4096) 
            : guild 
                ? CDN.GetGuildMemberAvatarUrl(_context.GuildID.Value, user.ID, hash!, imageSize: 4096)
                : CDN.GetUserAvatarUrl(user, imageSize: 4096);

        if (!avatar.IsDefined(out var avatarUrl))
        {
            return await _interactions.EditOriginalInteractionResponseAsync
                (
                 _context.ApplicationID,
                 _context.Token,
                 "Something went wrong while fetching that user's avatar!"
                );
        }
        
        return await _interactions.EditOriginalInteractionResponseAsync
            (
             _context.ApplicationID,
             _context.Token,
             embeds: new Embed[]
             {
                 new() 
                 {
                     Title    = $"{user.Username}'s avatar",
                     Colour  = Color.DodgerBlue,
                     Image = new EmbedImage(avatarUrl.ToString())
                 } 
             }
            );
    }
}