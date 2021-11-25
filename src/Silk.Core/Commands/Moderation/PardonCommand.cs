﻿using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Silk.Core.Data.Entities;
using Silk.Core.Services.Interfaces;
using Silk.Core.Types;
using Silk.Core.Utilities;
using Silk.Core.Utilities.HelpFormatter;
using Silk.Extensions.DSharpPlus;

namespace Silk.Core.Commands.Moderation
{
	[Category(Categories.Mod)]
	public class PardonCommand : BaseCommandModule
	{
		private readonly IInfractionService _infractions;
		public PardonCommand(IInfractionService infractions)
		{
			_infractions = infractions;
		}

		[Command("pardon")]
		[RequireFlag(UserFlag.Staff)]
		[Description("Pardon's a user from their last applicable infraction. \nThis will de-escalate the next infraction auto-mod, or escalated strike.\nThis will not undo mutes or bans.")]
		public async Task PardonAsync(CommandContext ctx, DiscordUser user, [RemainingText] string reason = "Not Given.")
		{
			if (ctx.User == user)
			{
				await ctx.RespondAsync("As much as I'd love to, I can't let you pardon yourself! Good manners though *:)*");
				return;
			}
			InfractionResult res = await _infractions.PardonAsync(user.Id, ctx.Guild.Id, ctx.User.Id, reason);

			if (res is InfractionResult.FailedGenericRequirementsNotFulfilled)
				await ctx.RespondAsync("Hmm. Seems that user doesn't have any infractions to be pardoned from! They should keep it up.");
			else
				await ctx.RespondAsync($"🚩 Pardoned **{user.ToDiscordName()}**. {(res is InfractionResult.SucceededWithNotification ? "(User notified with direct message)" : "(Failed to DM)")}");
		}
	}
}