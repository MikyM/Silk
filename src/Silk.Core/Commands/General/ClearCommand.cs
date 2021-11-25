﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using MediatR;
using Silk.Core.Data.MediatR.Guilds;
using Silk.Core.Data.Entities;
using Silk.Core.Utilities.HelpFormatter;
using Silk.Shared.Constants;

namespace Silk.Core.Commands.General
{
	[Category(Categories.Mod)]
	public class ClearCommand : BaseCommandModule
	{
		private readonly IMediator _mediator;

		public ClearCommand(IMediator mediator)
		{
			_mediator = mediator;
		}

		[Command]
		[Description("Cleans all messages from all users.")]
		[RequireUserPermissions(Permissions.ManageMessages)]
		public async Task Clear(CommandContext ctx, int numOfMessages = 5)
		{
			IReadOnlyList<DiscordMessage> queriedMessages = await ctx.Channel.GetMessagesAsync(numOfMessages + 1);

			var commandIssuingUser = $"{ctx.User.Username}{ctx.User.Discriminator}";
			await ctx.Channel.DeleteMessagesAsync(queriedMessages, $"{commandIssuingUser} called clear command.");

			DiscordEmbedBuilder? responseEmbed = MakeResponseEmbed(ctx, numOfMessages);
			DiscordMessage responseMsg = await ctx.RespondAsync(responseEmbed);

			GuildModConfigEntity guildConfig = await GetOrCreateGuildConfig(ctx);
			DiscordChannel? loggingChannel = ctx.Guild.GetChannel(guildConfig.LoggingChannel);

			DiscordEmbedBuilder? clearedMessagesEmbed = MakeLoggingChannelEmbed(ctx, numOfMessages);
			if (loggingChannel is not null) await loggingChannel.SendMessageAsync(clearedMessagesEmbed);

			await Task.Delay(5000);
			try { await ctx.Channel.DeleteMessageAsync(responseMsg); }
			catch (NotFoundException) { }
		}

		private static DiscordEmbedBuilder MakeResponseEmbed(CommandContext ctx, int messages)
		{
			return new DiscordEmbedBuilder()
				.WithAuthor(ctx.Member.DisplayName, null, ctx.Member.AvatarUrl)
				.WithColor(DiscordColor.SpringGreen)
				.WithDescription($"Cleared {messages} messages!");
		}

		private static DiscordEmbedBuilder MakeLoggingChannelEmbed(CommandContext ctx, int messages)
		{
			return new DiscordEmbedBuilder()
				.WithTitle("Cleared Messages:")
				.WithDescription(
					$"User: {ctx.User.Mention}\n" +
					$"Channel: {ctx.Channel.Mention}\n" +
					$"Amount: **{messages}**")
				.AddField("User ID:", ctx.User.Id.ToString(), true)
				.WithThumbnail(ctx.Message.Author.AvatarUrl)
				.WithFooter("Cleared Messages at (UTC)")
				.WithTimestamp(DateTime.Now.ToUniversalTime())
				.WithColor(DiscordColor.Red);
		}

		private async Task<GuildModConfigEntity> GetOrCreateGuildConfig(CommandContext ctx)
		{
			GuildEntity guild = await _mediator.Send(new GetOrCreateGuildRequest(ctx.Guild.Id, StringConstants.DefaultCommandPrefix));
			return guild.ModConfig;
		}
	}
}