﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FuzzySharp;
using Microsoft.Extensions.Caching.Memory;
using Silk.Core.Data.Entities;
using Silk.Core.Services.Server;
using Unity;

namespace Silk.Core.SlashCommands.ChoiceProviders
{
	public class TagChoiceProvider : IAutocompleteProvider
	{
		private readonly TagService _tags;

		private readonly IMemoryCache _cache;
		
		public TagChoiceProvider()
		{
			_tags = Startup.Container.Resolve<TagService>();
			_cache = Startup.Container.Resolve<IMemoryCache>();

		}
		
		public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
		{
			// This is possibly inefficient. //
			TagEntity[] tags = await GetTagsAsync(ctx);

			var query = ctx.OptionValue.ToString()!;

			var results = Process.ExtractSorted(new() { Name = query }, tags, s => s.Name).Where(r => r.Score > 40);
			
			return results.Select(r => new DiscordAutoCompleteChoice($"{r.Value.Name} ({r.Score}% match)", r.Value.Name));
		}
		
		private async Task<TagEntity[]> GetTagsAsync(AutocompleteContext ctx)
		{
			TagEntity[] tags;
			if (!_cache.TryGetValue($"guild_{ctx.Interaction.GuildId}_tags", out var tagsObj))
			{
				var dbTags = await _tags.GetGuildTagsAsync(ctx.Interaction.Guild.Id);
				tags = dbTags.ToArray();

				_cache.Set($"guild_{ctx.Interaction.GuildId}_tags", tags);
			}
			else
			{
				tags = (tagsObj as TagEntity[])!;
			}
			return tags;
		}
	}
}