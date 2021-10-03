﻿using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Silk.Core.EventHandlers.Guilds
{
	public sealed class GuildEventHandler
	{
		private readonly GuildCacher _guildHandler;
		public GuildEventHandler(GuildCacher guildHandler)
		{
			_guildHandler = guildHandler;
		}

		public async Task OnGuildJoin(DiscordClient client, GuildCreateEventArgs args)
		{
			_guildHandler.CacheQueue.Enqueue(() => _guildHandler.JoinedGuild(args));
		}

		public async Task OnGuildAvailable(DiscordClient client, GuildCreateEventArgs args)
		{
			_guildHandler.CacheQueue.Enqueue(() => _guildHandler.CacheGuildAsync(args.Guild, client.ShardId));
		}
	}
}