﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Extensions.Options;
using Remora.Commands.Attributes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Results;
using Silk.Commands.Conditions;
using Silk.Commands.Furry.Types;
using Silk.Shared.Configuration;
using Silk.Utilities.HelpFormatter;

namespace Silk.Commands.Furry;


[NSFWChannel]
[Category(Categories.Misc)]
public class e926Command : eBooruBaseCommand
{
    private readonly SilkConfigurationOptions _options;

    public e926Command(
        HttpClient                         client,
        IOptions<SilkConfigurationOptions> options,
        ICommandContext                    context,
        IDiscordRestChannelAPI             channel)
        : base(client, context, channel)
    {
        baseUrl  = "https://e926.net/posts.json?tags=";
        _options = options.Value;
    }
    
    [Command("e926", "e9")]
    [Description("Get cute furry content from e926.net")]
    public Task<IResult> Search
    (
        [Greedy]
        [Description("What tags to search for")]
        string query
    )
        => Search(3, query);
    
    //[RequireNsfw]
    [Command("e926", "e9")]
    [Description("Get cute furry content from e926.net")]
    public override async Task<IResult> Search
    (
        [Description("How many posts to show")]
        int amount = 3,
        
        [Description("what tags to search for")]
        string? query = null
    )
    {
        if (query?.Split().Length > 5)
            return Result.FromError(new ArgumentOutOfRangeError("You can search 5 tags at a time!"));

        if (amount > 10)
            return Result.FromError(new ArgumentOutOfRangeError("You can only request 10 images every 10 seconds."));

        Result<eBooruPostResult?> result;
        if (string.IsNullOrWhiteSpace(username))
            result = await QueryAsync(query); // May return empty results locked behind API key //
        else
            result = await AuthorizedQueryAsync(query, _options.E621.ApiKey, true);

        if (!result.IsSuccess)
        {
            Result<IMessage> errorResult = await _channelApi.CreateMessageAsync(_context.ChannelID, result.Error!.Message);
           return (Result)errorResult;
        }

        List<Post>? booruPosts = result.Entity?.Posts;

        if (booruPosts is null || booruPosts.Count is 0)
            return Result.FromError(new ArgumentOutOfRangeError("No results found."));

        IReadOnlyList<Post> posts = GetRandomPosts(booruPosts, amount, (int)((_context as MessageContext)?.MessageID.Value ?? 0));
        List<IEmbed> embeds = posts.Select(post =>
                                               new Embed
                                               {
                                                   Title       = query!,
                                                   Description = $"[Direct Link]({post!.File.Url})\nDescription: {post!.Description.Truncate(200)}",
                                                   Colour      = Color.RoyalBlue,
                                                   Image       = new EmbedImage(post.File.Url.ToString()),
                                                   Fields = new IEmbedField[]
                                                   {
                                                       new EmbedField("Score:", post.Score.Total.ToString(), true),
                                                       new EmbedField("Source:", GetSource(post.Sources.FirstOrDefault()?.ToString()) ?? "No source available", true)
                                                   }
                                               }
                                          )
                                   .Cast<IEmbed>()
                                   .ToList();

        Result<IMessage> send = await _channelApi.CreateMessageAsync(_context.ChannelID, embeds: embeds);

       return (Result)send;
    }
}