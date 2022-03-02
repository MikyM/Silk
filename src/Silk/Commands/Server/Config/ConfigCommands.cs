﻿//TODO: This

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Humanizer;
using MediatR;
using OneOf;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using Remora.Rest.Core;
using Remora.Results;
using Silk.Data.Entities;
using Silk.Data.MediatR.Guilds;
using Silk.Data.MediatR.Guilds.Config;
using Silk.Extensions;
using Silk.Extensions.Remora;
using Silk.Services.Data;
using Silk.Shared.Constants;
using Silk.Utilities.HelpFormatter;

namespace Silk.Commands.Server;

[Group("config", "cfg")]
[HelpCategory(Categories.Server)]
[RequireContext(ChannelContext.Guild)]
[Description("Configure various settings for your server!")]
[RequireDiscordPermission(DiscordPermission.ManageMessages, DiscordPermission.KickMembers, Operator = LogicalOperator.And)]
public partial class ConfigCommands : CommandGroup
{
    private readonly ICommandContext         _context;
    private readonly GuildConfigCacheService _configCache;
    private readonly IDiscordRestChannelAPI  _channels;

    public ConfigCommands(ICommandContext context, GuildConfigCacheService configCache, IDiscordRestChannelAPI channels)
    {
        _context     = context;
        _configCache = configCache;
        _channels    = channels;
    }

    [Command("reload")]
    [Description("Reloads the configuration for your server.")]
    public async Task<Result<IMessage>> ReloadConfigAsync()
    {
        _configCache.PurgeCache(_context.GuildID.Value);
        
        // If this fails it doesn't matter. Don't even await it.
        _ = _channels.CreateReactionAsync(_context.ChannelID, (_context as MessageContext)!.MessageID, $"_:{Emojis.ConfirmId}");
        
        return await _channels.CreateMessageAsync(_context.ChannelID, $"{Emojis.ReloadEmoji} Config reloaded! Changes should take effect immediately.");
    }

    [Group("view", "v")]
    [Description("View the settings for your server.")]
    public partial class ViewConfigCommands : CommandGroup
    {
        private readonly IMediator              _mediator;
        private readonly MessageContext         _context;
        private readonly IDiscordRestGuildAPI   _guilds;
        private readonly IDiscordRestChannelAPI _channels;

        public ViewConfigCommands
        (
            IMediator              mediator,
            MessageContext         context,
            IDiscordRestGuildAPI   guilds,
            IDiscordRestChannelAPI channels
        )
        {
            _mediator = mediator;
            _context  = context;
            _guilds   = guilds;
            _channels = channels;
        }
        
        //TODO: Add exmemptions
        
        [Command("all", "a")]
        [Description("View all settings for your server.")]
        public async Task<IResult> ViewAllAsync()
        {
            var config    = await _mediator.Send(new GetGuildConfig.Request(_context.GuildID.Value));
            var modConfig = await _mediator.Send(new GetGuildModConfig.Request(_context.GuildID.Value));

            var guildResult = await _guilds.GetGuildAsync(_context.GuildID.Value);

            if (!guildResult.IsDefined(out var guild))
                return guildResult;

            var phishingAction = modConfig.NamedInfractionSteps!.TryGetValue(AutoModConstants.PhishingLinkDetected, out var action) ? action.Type.ToString() : "Not configured";
            
            var contentBuilder = new StringBuilder();

            contentBuilder
               .Clear()
               .AppendLine($"{Emojis.SettingsEmoji} **General Config:**")
               .AppendLine("__Greetings__ | `config edit greetings`")
               .AppendLine($"> Configured Greetings: {config.Greetings.Count}")
               .AppendLine()
               .AppendLine($"{Emojis.WrenchEmoji} **Moderation Config:**")
               .AppendLine()
               .AppendLine("__Logging__ | Soon:tm:")
               .AppendLine($"> {(modConfig.Logging.LogMemberJoins ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.JoinEmoji} Log members joining")
               .AppendLine($"> {(modConfig.Logging.LogMemberLeaves ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.LeaveEmoji} Log members leaving")
               .AppendLine($"> {(modConfig.Logging.LogMessageEdits ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.EditEmoji} Log message edits")
               .AppendLine($"> {(modConfig.Logging.LogMessageDeletes ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.DeleteEmoji} Log message deletes")
               .AppendLine()
               .AppendLine("__Invites__ | `config edit invites`, `config edit invite-whitelist`")
               .AppendLine($"> {(modConfig.Invites.ScanOrigin ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.ScanEmoji} Scan invite origin")
               .AppendLine($"> {(modConfig.Invites.WarnOnMatch ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.WarningEmoji} Warn on invite")
               .AppendLine($"> {(modConfig.Invites.DeleteOnMatch ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.DeleteEmoji} Delete matched invite")
               .AppendLine($"> {(modConfig.UseAggressiveRegex ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.NoteEmoji} Use aggressive invite matching")
               .AppendLine($"> Allowed invites: {(modConfig.Invites.Whitelist.Count is 0 ? "None" : $"{modConfig.Invites.Whitelist.Count} allowed invites [See `config view invites`]")}")
               .AppendLine()
               .AppendLine("__Infractions__ | `config edit infractions`")
               .AppendLine($"> Mute role: {(modConfig.MuteRoleID.Value is 0 ? "Not set" : $"<@&{modConfig.MuteRoleID}>")}")
               .AppendLine($"> {(modConfig.UseNativeMute ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} Use native mutes (Requires Timeout Members permission)")
               .AppendLine($"> {(modConfig.ProgressiveStriking ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} Escalate infractions")
               .AppendLine($"> Infraction steps: {(modConfig.InfractionSteps.Count is var dictCount and not 0 ? $"{dictCount} steps [See `config view infractions`]" : "Not configured")}")
               .AppendLine($"> Infraction steps (named): {((modConfig.NamedInfractionSteps?.Count ?? 0) is var infNameCount and not 0 ? $"{infNameCount} steps [See `config view infractions`]" : "Not configured")}")
               .AppendLine()
               .AppendLine($"__Anti-Phishing__ {Emojis.NewEmoji} | `config edit phishing`")
               .AppendLine($"> {(modConfig.DetectPhishingLinks ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.WarningEmoji} Detect Phishing Links")
               .AppendLine($"> {(modConfig.DeletePhishingLinks ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.DeleteEmoji} Delete Phishing Links")
               .AppendLine($"> {(action is not null ? Emojis.EnabledEmoji : Emojis.DisabledEmoji)} {Emojis.WrenchEmoji} Post-detection action: {phishingAction}");

            var embed = new Embed
            {
                Title       = $"Config for {guild.Name}!",
                Colour      = Color.Goldenrod,
                Description = contentBuilder.ToString()
            };

            return await _channels.CreateMessageAsync(_context.ChannelID, embeds: new[] { embed });
        }


    }
}