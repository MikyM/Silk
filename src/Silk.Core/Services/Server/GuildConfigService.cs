﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ConcurrentCollections;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Silk.Core.Services.Interfaces;
using Silk.Shared.Types.Collections;

namespace Silk.Core.Services.Server
{
	/// <summary>
	/// A service class that provides API and backend functionality for per-server configuration UI handling.
	/// </summary>
	/* TODO: Finish the implementation of this. Will probably need to get some help tbqh. */
	public sealed class GuildConfigService
	{
		#region Class Initialization
		
		private readonly DiscordShardedClient _client;
		private readonly ILogger<GuildConfigService> _logger;
		private readonly ConcurrentHashSet<ulong> _activeMenus = new();
		private readonly IServiceCacheUpdaterService _updater;
		private readonly ConfigService _config;

		private readonly LoopedList<DiscordButtonComponent> _greetingEnableToggle = new()
		{
			new(ButtonStyle.Danger, $"{Config}{Button}{Greeting}{Edit}{Toggle}", "Disabled"),
			new(ButtonStyle.Success, $"{Config}{Button}{Greeting}{Edit}{Toggle}", "Enabled")
		};
		

		private const string 
			Config = "cf",
			Button = "bt",
			Dropdown = "dd",
			Greeting = "gt",
			View = "vw",
			Edit = "ed",
			Main = "mn",
			Toggle = "tg";

		private readonly Dictionary<string, Func<GuildConfigService, DiscordInteraction, Task>> _compMethDict = new()
		{
			[$"{Config}{Dropdown}"] = (g, i) => g.HandleDropdownAsync(i),
			/* TODO: Implement main menu because I'm somehow this stupid */
			[$"{Config}{Main}"] = (_, _) => Task.CompletedTask, 
			[$"{Config}{Button}{Greeting}{View}"] = (g, i) => g.ViewCurrentGreetingAsync(i),
			[$"{Config}{Button}{Greeting}{Edit}"] = (g, i) => g.EditCurrentGreetingAsync(i),
			[$"{Config}{Button}{Greeting}{Edit}{Toggle}"] = (g, i) => g.ToggleGreetingAsync(i)
		};
		
		public GuildConfigService(DiscordShardedClient client, ILogger<GuildConfigService> logger, ConfigService config, IServiceCacheUpdaterService updater)
		{
			_client = client;
			_logger = logger;
			_config = config;
			_updater = updater;
			_client.ComponentInteractionCreated += HandleComponentAsync;

		}
		#endregion
		
		
		/// <summary>
		/// Presents a view for the configuration of the provided guild's Id.
		/// </summary>
		/// <param name="interaction">The slash command context to respond with.</param>
		public async Task ViewCurrentServerConfig(InteractionContext interaction) { }


		#region Event Dispatching

		private async Task HandleComponentAsync(DiscordClient sender, ComponentInteractionCreateEventArgs e)
		{
			if (!e.Id.StartsWith(Config))
				return;
			await e.Interaction.CreateResponseAsync(InteractionResponseType.DefferedMessageUpdate);
			
			e.Handled = true;
			_activeMenus.Add(e.Message.Id);

			if (_compMethDict.TryGetValue(e.Id, out var me))
			{
				await me(this, e.Interaction);
				return;
			}

			await e.Interaction.CreateFollowupMessageAsync(new()
			{
				IsEphemeral = true,
				Content = "Sorry, but that doesn't have implemented functionality! Please contact the developers immediately."
			});
		}
		
		private Task HandleDropdownAsync(DiscordInteraction args) 
			=> args.EditOriginalResponseAsync(new() {Content = "Oh no, this hasn't been implemented yet!"});

		#endregion

		#region Welcome / Greeting
		
		/// <summary>
		/// Allows the user to view or edit the current server greeting, if configured.
		/// </summary>
		/// <param name="interaction">The interaction to edit.</param>
		public async Task ShowWelcomeScreenAsync(DiscordInteraction interaction)
		{
			var builder = new DiscordWebhookBuilder();
			var currentConfig = await _config.GetConfigAsync(interaction.GuildId.Value); // This is cached in memory. //
			
			var components = new[]
			{
				new DiscordButtonComponent(ButtonStyle.Primary, $"{Config}{Button}{Greeting}{View}", "View current greeting config", !currentConfig.GreetMembers),
				new DiscordButtonComponent(ButtonStyle.Secondary, $"{Config}{Button}{Greeting}{Edit}", "Edit current greeting config")
			};
			
			builder.WithContent("Please make a selection.");
			builder.AddComponents(components);
			
			await interaction.EditOriginalResponseAsync(builder);
		}

		private async Task ViewCurrentGreetingAsync(DiscordInteraction interaction)
		{
			var currentConfig = await _config.GetConfigAsync(interaction.GuildId.Value);
			
			// This shouldn't be possible, since the button that invokes this command is
			//disabled when the config isn't set to greet members, but we should check again anyways.
			if (!currentConfig.GreetMembers)
			{
				var builder = new DiscordWebhookBuilder();
				builder.WithContent("Sorry, but this server isn't set up to greet members...yet.");
				builder.AddComponents(
					new DiscordButtonComponent(ButtonStyle.Secondary, $"{Config}{Main}", "Return to main config menu"),
					new DiscordButtonComponent(ButtonStyle.Secondary, $"{Config}{Button}{Greeting}{View}", "View current greeting config"));
					
				await interaction.EditOriginalResponseAsync(builder);
				
			}
		}

		private async Task EditCurrentGreetingAsync(DiscordInteraction interaction)
		{
			var currentConfig = await _config.GetConfigAsync(interaction.GuildId.Value);
			var builder = new DiscordWebhookBuilder();
			builder.WithContent("Greeting config:");
			var components = new[]
			{
				new DiscordButtonComponent(currentConfig.GreetMembers ? ButtonStyle.Success : ButtonStyle.Danger, $"{Config}{Button}{Greeting}{Edit}{Toggle}", currentConfig.GreetMembers ? "Enabled" : "Disabled"),
			};

			builder.AddComponents(components);
			await interaction.EditOriginalResponseAsync(builder);
		}
		
		private async Task ToggleGreetingAsync(DiscordInteraction interaction)
		{
			var msg = (DiscordMessage)typeof(DiscordInteraction)
				.GetProperty("Message", BindingFlags.NonPublic | BindingFlags.Instance)!
				.GetValue(interaction)!;

			var cmps = msg.Components.First().Components.ToArray();
			var btn = (cmps[0] as DiscordButtonComponent)!;
			
			var style = Unsafe.As<DiscordButtonComponent>(cmps[0]).Style == ButtonStyle.Danger ? ButtonStyle.Success : ButtonStyle.Danger;
			
			var text = style is ButtonStyle.Danger ? "Disabled" : "Enabled";

			var builder = new DiscordWebhookBuilder();
			var bcmps = cmps.Skip(1).Prepend(new DiscordButtonComponent(style, btn.CustomId, text));

			builder.WithContent("Greeting config:");
			builder.AddComponents(bcmps);

			await interaction.EditOriginalResponseAsync(builder);
		}

		#endregion
		
		
	}
}