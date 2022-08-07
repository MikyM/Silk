﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Results;
using Remora.Discord.Interactivity;
using Remora.Rest.Core;
using Remora.Results;
using RoleMenuPlugin.Conditions;
using RoleMenuPlugin.Database;
using RoleMenuPlugin.Database.MediatR;
using Silk.Shared;

// ReSharper disable once ContextualLoggerProblem
// ReSharper disable RedundantBlankLines
namespace RoleMenuPlugin;

/// <summary>
/// The command module responsible for creating, modifying, and deleting role menus.
/// </summary>
[PublicAPI]
[Group("rolemenu", "rm")]
[RequireContext(ChannelContext.Guild)]
[RequireDiscordPermission(DiscordPermission.ManageChannels)]
[Description("Role Menu related commands.\n" +
             "V3 has simplified and streamlined this process, using a command-based creation and modification system. \n\n" +
             "**Quick-start guilde**:\n" +
             "`rolemenu create` - Creates a new menu, optionally adding roles to it.\n" +
             "`rolemenu add` - Adds roles to a role menu")]
public sealed class RoleMenuCommand : CommandGroup
{
    private readonly IMediator                _mediator;
    private readonly MessageContext           _context;
    private readonly IDiscordRestUserAPI      _users;
    private readonly IDiscordRestGuildAPI     _guilds;
    private readonly IDiscordRestChannelAPI   _channels;
    private readonly ILogger<RoleMenuCommand> _logger;
    
    public RoleMenuCommand
    (
        IMediator                mediator,
        MessageContext           context,
        IDiscordRestUserAPI      users,
        IDiscordRestGuildAPI     guilds,
        IDiscordRestChannelAPI   channels,
        ILogger<RoleMenuCommand> logger
    )
    {
        _context  = context;
        _users    = users;
        _channels = channels;
        _guilds   = guilds;
        _logger   = logger;
        _mediator = mediator;
    }

    [Command("create", "c")]
    [Description("Initiates a role menu creation session. Roles can be added at this stage via mentions (@Role) or IDs (123456789).\n" +
                 "It's recommended to create a role menu in a channel users cannot view whilst setting up the role menu.\n" +
                 "The role menu message is sent immediately and updated as you update your role menu, which may confuse vigilant users!")]
    public async Task<IResult> CreateAsync
    (
        [Description("The channel the role menu will be created in.\n" +
                     "This channel must be a text channel, and must allow sending messages.")]
        [RequireBotDiscordPermissions(DiscordPermission.SendMessages)]
        IChannel channel,
        
        [Description("The roles to add to the role menu. This is optional!\n" +
                     "Any roles above my own and the @everyone role will be discarded!")]
        IRole[]? roles = null
    )
    {
        var permsResult = await EnsureChannelPermissionsAsync(channel);

        if (permsResult is not Result<(IReadOnlyList<IRole>, IRole)> permissionResultWithValue)
            return permsResult;

        var (selfRoles, everyoneRole) = permissionResultWithValue.Entity;

        roles = (roles ?? Array.Empty<IRole>())
               .DistinctBy(r => r.ID)
               .Where(r => r.Position <= selfRoles.Max(sr => sr.Position))
               .Except(new[] {everyoneRole})
               .ToArray();

        var roleMenuResult = await CreateRoleMenuMessageAsync(channel.ID, roles);

        if (!roleMenuResult.IsSuccess)
            return Result<ReactionResult>.FromError(new ReactionResult("❌", "Failed to create role menu: " + roleMenuResult.Error!.Message), roleMenuResult);

        await _channels.CreateMessageAsync(_context.ChannelID,
                                               $"RoleMenu ID: {((Result<IMessage>) roleMenuResult).Entity.ID}.\n" +
                                               $"Use this ID to edit the menu!");


        return Result<ReactionResult>.FromSuccess(new("✅"));
    }

    [Command("add", "a")]
    [Description("Adds one or more role(s) to the role menu.\n" +
                 "Roles can be added at this stage via mentions (@Role) or IDs (123456789).\n")]
    public Task<IResult> AddAsync
    (
        [RoleMenu]
        [Description("A link to the role menu message. \n"                                                                +
                     "This can be obtained by right clicking or holding the message and selecting `Copy Message Link`.\n" +
                     "**This is required if you set the role menu to the current channel.**")]
        IMessage message,
            
        [Description("The roles to add to the role menu. Roles above my own and the @everyone role will be discarded!")]
        IRole[] roles
    )
    => AddAsync(message.ID, roles);
    
    [Command("add", "a")]
    [Description("Adds one or more role(s) to the role menu.\n"                                             +
                 "Roles can be added at this stage via mentions (@Role) or IDs (123456789)")]
    public async Task<IResult> AddAsync
    (
        [RoleMenu]
        [Description("The ID of the role menu message, provided when creating the role menu.")]
        Snowflake messageID,
        
        [Description("The roles to add to the role menu. Roles above my own and the @everyone role will be discarded!")]
        IRole[] roles
    )
    {
        if (!roles.Any())
        {
           await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
           return await DeleteAfter(_context, _channels, "You must provide at least one role!", TimeSpan.FromSeconds(5));
        }

        var roleMenuResult = await _mediator.Send(new GetRoleMenu.Request(messageID.Value));

        var roleResult = await GetRolesAsync();

        if (!roleResult.IsSuccess)
        {
            await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
            return await DeleteAfter(_context, _channels, "There was an internal error while processing that. Sorry.", TimeSpan.FromSeconds(12));
        }

        var (everyoneRole, allRoles) = roleResult.Entity;

        var roleMenu = roleMenuResult.Entity;

        var duplicateRoles = roles.Where(r => roleMenu.Options.Select(rm => rm.RoleId).Contains(r.ID.Value));
        
        var rolesToAdd = roles
                        .DistinctBy(r => r.ID)
                        .Where(r => r.Position <= allRoles.Max(sr => sr.Position))
                        .Except(new[] {everyoneRole})
                        .Except(duplicateRoles)
                        .ToArray();

        if (duplicateRoles.Count() == roles.Length)
        {
            await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
            return await DeleteAfter(_context, _channels, "All the roles you're trying to add are already added!", TimeSpan.FromSeconds(5));
        }
        
        if (!rolesToAdd.Any())
        {
            await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
            return await DeleteAfter(_context, _channels, "It appears all the roles you gave me are above mine or can't be applied.\n\n" +
                                                          "Consider moving my role above them first?", TimeSpan.FromSeconds(12));
        }

        if (rolesToAdd.Length + roleMenu.Options.Count > 25)
        {
            await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");

            return await
                DeleteAfter(_context, _channels, $"You can only have 25 roles in a role menu! You're {roles.Length + roleMenu.Options.Count - 25} roles over!",
                            TimeSpan.FromSeconds(15));
        }

        roleMenu.Options.AddRange(rolesToAdd.Select(r => new RoleMenuOptionModel
        {
            RoleMenuId = roleMenu.MessageId,
            MessageId  = roleMenu.MessageId,
            GuildId    = roleMenu.GuildId,
            RoleId     = r.ID.Value,
            RoleName   = r.Name
        }));
        
        await _mediator.Send(new UpdateRoleMenu.Request(messageID, roleMenu.Options));

        var components = new ActionRowComponent(new[] { new ButtonComponent(ButtonComponentStyle.Success, "Get Roles!", CustomID: CustomIDHelpers.CreateButtonID(RoleMenuInteractionCommands.RoleMenuButtonPrefix), IsDisabled: false) });
        
        var roleMenuMessageResult = await _channels.EditMessageAsync
        (
         new(roleMenu.ChannelId),
         messageID,
         "**Role Menu!**\n"                               +
         "Use the button below to select your roles!\n\n" +
         "Available roles:\n"                             +
         string.Join('\n', roleMenu.Options.Select(r => $"<@&{r.RoleId}>")),
         components: new[] {components}
        );

        if (roleMenuMessageResult.IsSuccess)
            return Result<ReactionResult>.FromSuccess(new("✅"));
        
        _logger.LogWarning($"Failed to edit role menu message {roleMenu.ChannelId}/{roleMenu.MessageId}.");
        await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
            
        return await DeleteAfter(_context, _channels, "I couldn't edit the role menu message, are you sure it still exists?", TimeSpan.FromSeconds(10));

    }


    [Group("edit", "e")]
    [Description("Edits aspects about a role menu such as max options, or required roles.")]
    public class EditGroup : CommandGroup
    {
        private readonly IMediator              _mediator;
        private readonly MessageContext         _context;
        private readonly IDiscordRestChannelAPI _channels;
        
        public EditGroup(IMediator mediator, MessageContext context, IDiscordRestChannelAPI channels)
        {
            _mediator      = mediator;
            _context       = context;
            _channels = channels;
        }

        [Command("menu", "m")]
        [Description("Edits the role menu message.\n" +
                     "This can be obtained by right clicking or holding the message and selecting `Copy Message Link`.\n" +
                     "**This is required if you set the role menu to the current channel.**")]
        public Task<IResult> EditAsync
        (
            [RoleMenu]
            [Description("A link to the role menu message. \n"                                                                +
                         "This can be obtained by right clicking or holding the message and selecting `Copy Message Link`.\n" +
                         "**This is required if you set the role menu to the current channel.**")]
            IMessage message,
            
            [Range(Min = 1, Max = 25)]
            [Description("The maximum number of roles that can be selected.")]
            int maxOptions
        )
        => EditAsync(message.ID, maxOptions);
        
        [Command("menu", "m")]
        [Description("Edits the role menu message.\n"                                                                     +
                     "This can be obtained by right clicking or holding the message and selecting `Copy Message Link`.\n" +
                     "**This is required if you set the role menu to the current channel.**")]
        public async Task<IResult> EditAsync
        (
            [RoleMenu]
            [Description("The ID of the role menu message, provided when creating the role menu.")]
            Snowflake messageID,

            
            [Range(Min = 1, Max = 25)]
            [Description("The maximum number of roles that can be selected.")]
            int maxOptions
        )
        {
            var roleMenuResult = await _mediator.Send(new GetRoleMenu.Request(messageID.Value));
            
            var res = await _mediator.Send(new UpdateRoleMenu.Request(messageID, roleMenuResult.Entity.Options, maxOptions));

            if (res.IsSuccess)
                return Result<ReactionResult>.FromSuccess(new("✅"));
            
            await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
            return await DeleteAfter(_context, _channels, "There was an internal error while processing that. Sorry.", TimeSpan.FromSeconds(12));
        }

        [Command("role", "r")]
        [Description("Edits various attributes of a role menu option.")]
        public Task<IResult> EditAsync
        (
            [RoleMenu]
            [Description("A link to the role menu message. \n"                                                                +
                         "This can be obtained by right clicking or holding the message and selecting `Copy Message Link`.\n" +
                         "**This is required if you set the role menu to the current channel.**")]
            IMessage message,
            
            [Description("The ID of the role menu option to edit.")]
            IRole role,
            
            [Range(Min = 0, Max = 25)]
            [Option('x', "exclusive")]
            [Description("What role should be mutually exclusive with this option?")]
            IRole[]? exclusiveRoles = null,

            [Range(Min = 0, Max = 25)]
            [Option('i', "inclusive")]
            [Description("What roles should be mutually inclusive with this option?")]
            IRole[]? inclusiveRoles = null,
            
            [Greedy]
            [Option('d', "description")]
            [Description("The description of the role menu option.")]
            string? description = null
        )
        => EditAsync(message.ID, role, exclusiveRoles, inclusiveRoles, description);

        [Command("role", "r")]
        [Description("Edits various attributes of a role menu option.")]
        public async Task<IResult> EditAsync
        (
            [RoleMenu]
            [Description("The ID of the role menu message, provided when creating the role menu.")] 
            Snowflake messageId,

            [Description("The ID of the role menu option to edit.")]
            IRole role,
            
            [Range(Min = 0, Max = 25)]
            [Option('x', "exclusive")]
            [Description("What role should be mutually exclusive with this option?")]
            IRole[]? exclusiveRoles = null,

            [Range(Min = 0, Max = 25)]
            [Option('i', "inclusive")]
            [Description("What roles should be mutually inclusive with this option?")]
            IRole[]? inclusiveRoles = null,
            
            [Greedy]
            [Option('d', "description")]
            [Description("The description of the role menu option.")]
            string? description = null
        )
        {
            var roleMenuResult = await _mediator.Send(new GetRoleMenu.Request(messageId.Value));
            
            var roleMenu = roleMenuResult.Entity;

            var roleMenuOption = roleMenu.Options.FirstOrDefault(r => r.RoleId == role.ID.Value);

            if (roleMenuOption == null)
            {
                await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
                return await DeleteAfter(_context, _channels, "I don't see that role in the role menu, are you sure it still exists?", TimeSpan.FromSeconds(5));
            }

            if (description != null)
                roleMenuOption.Description = description;

            if (exclusiveRoles != null)
            {
                if (exclusiveRoles.Contains(role))
                {
                    await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
                    return await DeleteAfter(_context, _channels, "You can't exclude a role from itself.", TimeSpan.FromSeconds(5));
                }

                roleMenuOption.MutuallyExclusiveRoleIds = exclusiveRoles
                                                         .ExceptBy(new[] { role.ID, _context.GuildID.Value }, r => r.ID)
                                                         .Select(r => r.ID.Value)
                                                         .ToArray();
            }

            if (inclusiveRoles != null)
            {
                if (inclusiveRoles.Contains(role))
                {
                    await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
                    return await DeleteAfter(_context, _channels, "You can't make a role depend on itself.", TimeSpan.FromSeconds(5));
                }
                
                roleMenuOption.MutuallyInclusiveRoleIds = inclusiveRoles
                                                         .ExceptBy(new[] { role.ID, _context.GuildID.Value }, r => r.ID)
                                                         .Select(r => r.ID.Value)
                                                         .ToArray();
            }

            if (inclusiveRoles?.Intersect(exclusiveRoles ?? Array.Empty<IRole>()).Any() ?? false)
            {
                await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
                return await DeleteAfter(_context, _channels, "You can't make a role mutually exclusive and inclusive.", TimeSpan.FromSeconds(5));
            }

            await _mediator.Send(new UpdateRoleMenu.Request(messageId, roleMenu.Options));

            return Result<ReactionResult>.FromSuccess(new("✅"));
        }
    }


    [Command("remove", "rm", "r")]
    [Description("Removes one or more roles from the role menu.")]
    public Task<IResult> RemoveAsync
    (
        [RoleMenu]
        [Description("A link to the role menu message. This can be obtained by right clicking or holding the message and selecting `Copy Message Link`.")]
        IMessage message,
        
        [Description("The roles to remove, roles can be removed by mention (@Role) or by ID (123456789).\n" +
                     "You must have at least one role in the role menu, however.")]
        IRole[] roles
    )
    => RemoveAsync(message.ID, roles);

    [Command("remove", "rm", "r")]
    [Description("Removes one or more roles from the role menu.")]
    public async Task<IResult> RemoveAsync
    (
        [RoleMenu]
        [Description("The ID of the role menu message.")]
        Snowflake messageID,

        [Range(Min = 1, Max = 25)]
        [Description("The roles to remove, roles can be removed by mention (@Role) or by ID (123456789).\n" +
                     "You must have at least one role in the role menu, however.")]
        IRole[] roles
    )
    {
        if (!roles.Any())
        {
            await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
            return await DeleteAfter(_context, _channels, "You must provide at least one role!", TimeSpan.FromSeconds(5));
        }
        
        var roleMenuResult = await _mediator.Send(new GetRoleMenu.Request(messageID.Value));

        var roleMenu = roleMenuResult.Entity;

        var roleOptions = roleMenu.Options;

        var newRoles = roleOptions.ExceptBy(roles.Select(r => r.ID.Value), o => o.RoleId).ToArray();
        
        if (!newRoles.Any())
        {
            await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
            
            return await DeleteAfter(_context, _channels, "It appears you're trying to clear the role menu." +
                                                          "\n\nPerhaps you meant to use `rolemenu delete` instead?", TimeSpan.FromSeconds(25));
        }
        
        await _mediator.Send(new UpdateRoleMenu.Request(messageID, newRoles));
        
        var roleMenuMessageResult = await _channels.EditMessageAsync
        (
         new(roleMenu.ChannelId),
         messageID,
         "**Role Menu!**\n"                              +
         "Use the button below to select your roles!\n\n" +
         "Available roles:\n"                             +
         string.Join('\n', newRoles.Select(r => $"<@&{r.RoleId}>"))
        );

        if (roleMenuMessageResult.IsSuccess)
            return Result<ReactionResult>.FromSuccess(new("✅"));
        _logger.LogWarning($"Failed to edit role menu message {roleMenu.ChannelId}/{roleMenu.MessageId}.");
        await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
            
        return await DeleteAfter(_context, _channels, "I couldn't edit the role menu message, are you sure it still exists?", TimeSpan.FromSeconds(10));

    }

    
    private static async Task<IResult> DeleteAfter(MessageContext context, IDiscordRestChannelAPI api,  string content, TimeSpan delay)
    {
        var sendResult = await api.CreateMessageAsync(context.ChannelID, content);

        if (!sendResult.IsSuccess)
            return sendResult;

        await Task.Delay(delay);

        return await api.DeleteMessageAsync(context.ChannelID, sendResult.Entity.ID);
    }
    
    private async Task<IResult> CreateRoleMenuMessageAsync(Snowflake channelID, IReadOnlyList<IRole> roles)
    {
        var roleMenuMessageResult = await _channels.CreateMessageAsync
        (
         channelID,
         !roles.Any()
             ? "This role menu is being set up, please wait!"
             : "**Role Menu!**\n"                               +
               "Use the button below to select your roles!\n\n" +
               "Available roles:\n"                             +
               string.Join('\n', roles.Select(r => $"<@&{r.ID}>")),
         components:
         new[]
         {
             new ActionRowComponent(new[]
             {
                 new ButtonComponent(ButtonComponentStyle.Success, "Get Roles!", CustomID: CustomIDHelpers.CreateButtonID(RoleMenuInteractionCommands.RoleMenuButtonPrefix), IsDisabled: !roles.Any())
             })
         },
         allowedMentions: new AllowedMentions
         (
            Parse: Array.Empty<MentionType>(),
            Roles: Array.Empty<Snowflake>()
         )
        );

        if (!roleMenuMessageResult.IsSuccess)
            return roleMenuMessageResult;

        var roleMenu = await _mediator.Send
            (
             new CreateRoleMenu.Request(new()
             {
                 ChannelId = channelID.Value,
                 GuildId = _context.GuildID.Value.Value,
                 MessageId = roleMenuMessageResult.Entity.ID.Value,
                 Options = roles.Select(r => new RoleMenuOptionModel
                                            {RoleId = r.ID.Value, RoleName = r.Name})
                                .ToList()
             })
            );

        if (!roleMenu.IsSuccess)
            return roleMenu;

        return roleMenuMessageResult;
    }

    
    [Command("delete", "d")]
    [Description("Deletes a role menu. This cannot be undone! For this reason, you must confirm that you actually want to delete the rolemenu.")]
    public Task<IResult> DeleteAsync
    (
        [RoleMenu]
        [Description("A link to the role menu you'd like to delete.")]
        IMessage message,
        
        [Switch("confirm")]
        [Description("Ensures you don't accidentally delete a role menu!")]
        bool confirm = false
    ) => DeleteAsync(message.ID, confirm);

    [Command("delete", "d")]
    [Description("Deletes a role menu. This cannot be undone! For this reason, you must confirm that you actually want to delete the rolemenu.")]
    public async Task<IResult> DeleteAsync
    (
        [RoleMenu]
        [Description("The ID of the role menu you'd like to delete.")]
        Snowflake messageID,

        [Switch("confirm")] [Description("Ensures you don't accidentally delete a role menu!")]
        bool confirm = false
    )
    {
        if (!confirm)
        {
            await _channels.CreateReactionAsync(_context.ChannelID, _context.MessageID, "❌");
            
            return await DeleteAfter(_context, _channels, "To ensure rolemenus aren't accidentally deleted, you must confirm this action by adding `--confirm` to the end of your command.", TimeSpan.FromSeconds(10));
        }

        var roleMenuResult = await _mediator.Send(new GetRoleMenu.Request(messageID.Value));
        
        await _mediator.Send(new DeleteRoleMenu.Request(messageID.Value));
        
        if (roleMenuResult.IsSuccess)
            await _channels.DeleteMessageAsync(new(roleMenuResult.Entity.ChannelId), new(roleMenuResult.Entity.MessageId));
        
        return Result<ReactionResult>.FromSuccess(new("✅"));
    }
    
    private async Task<IResult> EnsureChannelPermissionsAsync(IChannel channel)
    {
        var roleResult = await GetRolesAsync();

        if (!roleResult.IsSuccess)
            return roleResult;

        var (everyoneRole, selfRoles) = roleResult.Entity;

        var channelPermissionResult = DiscordPermissionSet
           .ComputePermissions(_context.User.ID,
                               everyoneRole,
                               selfRoles,
                               channel.PermissionOverwrites.Value);

        if (!channelPermissionResult.HasPermission(DiscordPermission.SendMessages))
        {
            await _channels.CreateMessageAsync(_context.ChannelID, "I can't send messages to that channel!");
            return Result.FromError(new PermissionDeniedError());
        }

        return Result<(IReadOnlyList<IRole>, IRole)>.FromSuccess((selfRoles, everyoneRole));
    }

    private async Task<Result<(IRole everyoneRole, IRole[] selfRole)>> GetRolesAsync()
    {
        var rolesResult = await _guilds.GetGuildRolesAsync(_context.GuildID.Value);

        if (!rolesResult.IsDefined(out var guildRoles))
            return Result<(IRole, IRole[])>.FromError(rolesResult.Error!);

        var selfResult = await _users.GetCurrentUserAsync();

        if (!selfResult.IsDefined(out var self))
            return Result<(IRole, IRole[])>.FromError(selfResult.Error!);

        var memberResult = await _guilds.GetGuildMemberAsync(_context.GuildID.Value, self.ID);

        if (!memberResult.IsDefined(out var member))
            return Result<(IRole, IRole[])>.FromError(memberResult.Error!);

        var everyoneRole = guildRoles.First(r => r.ID == _context.GuildID.Value);
        var selfRoles    = guildRoles.Where(r => member.Roles.Contains(r.ID)).ToArray();

        return Result<(IRole everyoneRole, IRole[] selfRole)>.FromSuccess((everyoneRole, selfRoles));
    }
}