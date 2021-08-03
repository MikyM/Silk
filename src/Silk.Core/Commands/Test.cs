﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Menus;
using DSharpPlus.Menus.Attributes;
using DSharpPlus.Menus.Entities;
using Silk.Core.Types;

namespace Silk.Core.Commands
{
	public class Test : BaseCommandModule
	{
		[Command]
		public async Task ViewConfig(CommandContext ctx)
		{
			var menu = new BaseConfigMenu(ctx.Client);
			await menu.StartAsync();
			await ctx.RespondAsync(m => m.WithContent("** **").AddMenu(menu));
			
		}

	}
	
	public class BaseConfigMenu : Menu
	{
		public BaseConfigMenu(DiscordClient client, TimeSpan? timeout = null) : base(client, timeout) { }
		
		private async Task Interact(ButtonContext ctx)
			=> await ctx.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddMenu(this).WithContent("** **"));


		private async Task ShowNewMenuAsync(ButtonContext ctx)
		{
			var menu = new ModConfigMenu(Client, this);
			await menu.StartAsync();
			await ctx.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddMenu(menu).WithContent("** **"));
		}

		// For this button to be registered it must have one of the button attributes,
		// have `ComponentInteractionCreateEventArgs` as first and only parameter and return `Task`
		[SuccessButton("Test two (this one does nothing)", Row = ButtonPosition.Third)]
		public Task SuccessAsync(ButtonContext ctx) => Interact(ctx);

		[PrimaryButton("Test one", Row = ButtonPosition.First)]
		public Task DangerAsync(ButtonContext ctx) => ShowNewMenuAsync(ctx);
		
	}

	public sealed class ModConfigMenu : StagedMenu
	{
		public ModConfigMenu(DiscordClient client, Menu previous , TimeSpan? timeout = null) : base(client, previous, timeout) { }
		
		[SecondaryButton("Back!")]
		public override Task BackAsync(ButtonContext ctx)
			=> ctx.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent(ctx.Message.Content).AddMenu(_previous));
		
		[SuccessButton("Test?")]
		public Task Handle(ButtonContext ctx) => ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

		
		[SuccessButton("Test2?")]
		public Task Handle2(ButtonContext ctx) => ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
	}
}