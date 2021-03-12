﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Humanizer;
using Humanizer.Localisation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Silk.Data.MediatR;
using Silk.Data.Models;
using Silk.Extensions;
using Timer = System.Threading.Timer;

namespace Silk.Core.Services
{
    public class ReminderService : BackgroundService
    {
        private const string MissingChannel = "Hey!, you wanted me to remind you of something, but the channel was deleted, or is otherwise inaccessible to me now.\n";
        
        private List<Reminder> _reminders; // We're gonna slurp all reminders into memory. Yolo, I guess.
        private readonly IServiceProvider _services;
        private readonly ILogger<ReminderService> _logger;

        private readonly DiscordShardedClient _client;

        private Timer _timer;
        public ReminderService(ILogger<ReminderService> logger, IServiceProvider services, DiscordShardedClient client)
        {
            _logger = logger;
            _services = services;
            _client = client;
        }

        public async Task CreateReminder
        (DateTime expiration, ulong ownerId, 
            ulong channelId, ulong messageId, ulong guildId,
            string messageContent, bool wasReply, ulong? replyId = null,
            ulong? replyAuthorId = null, string? replyMessageContent = null)
        {
            using IServiceScope scope = _services.CreateScope();
            var mediator = _services.CreateScope().ServiceProvider.Get<IMediator>();
            Reminder reminder = await mediator.Send(new ReminderRequest.Create(expiration, ownerId, channelId, messageId, guildId, messageContent, wasReply, replyId, replyAuthorId, replyMessageContent));
            _reminders.Add(reminder);
        }

        private async void Tick(object? state)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            // Collection gets modified. //
            for (int i = 0; i < _reminders.Count; i++)
            {
                Reminder r = _reminders[i];
                if (r.Expiration < DateTime.Now)
                    _ = SendReminderMessageAsync(r);
            }
               
        }

        private async Task SendReminderMessageAsync(Reminder reminder)
        {
            var guilds = _client.ShardClients.SelectMany(s => s.Value.Guilds);
            if (!(guilds.FirstOrDefault(g => g.Key == reminder.GuildId).Value is { } guild))
            {
                _logger.LogWarning("Couldn't find guild {GuildId}! Removing reminders from queue", (reminder.GuildId));
                _reminders.RemoveAll(r => r.GuildId == reminder.GuildId);
            }
            else
            {
                _logger.LogTrace("Dequeing reminder...");
                _reminders.Remove(reminder);
                if (!guild.Channels.TryGetValue(reminder.ChannelId, out var channel))
                {
                    _logger.LogTrace("Channel doesn't exist on guild! Attempting to DM user");
                    try
                    {
                        await (await guild.GetMemberAsync(reminder.OwnerId))
                            .SendMessageAsync(MissingChannel +
                                              $"{(DateTime.Now - reminder.Expiration).Humanize(2, minUnit: TimeUnit.Second)} ago: \n{reminder.MessageContent}");
                    }
                    catch (UnauthorizedException)
                    {
                        _logger.LogTrace("Failed to message user. Skipping reminder");
                    }
                    catch (NotFoundException)
                    {
                        _logger.LogTrace("Member left guild. Skipping");
                        return;
                    }
                }
                else
                {
                    _logger.LogTrace("Preparing to send reminder...");
                    var builder = new DiscordMessageBuilder().WithAllowedMention(new UserMention(reminder.OwnerId));
                    var mention = reminder.WasReply ? $"<@{reminder.OwnerId}>" : null;
                    var message = $"Hey, {mention}! {(DateTime.Now - reminder.Expiration).Humanize(2, minUnit: TimeUnit.Second)} ago:\n{reminder.MessageContent}";

                    if (reminder.WasReply)
                    {
                        bool validReply;
                        
                        try { validReply = await channel.GetMessageAsync(reminder.ReplyId.Value) is not null; }
                        catch (NotFoundException) { validReply = false; }
                        
                        if (validReply)
                        {
                            builder.WithReply(reminder.ReplyId.Value);
                            builder.WithContent(message);
                        }
                        else
                        {
                            message += "\n(You replied to someone, but that message was deleted!)\n";
                            message += $"Replying to:\n> {reminder.ReplyMessageContent!.Pull(..250)}\n" +
                                       $"From: <@{reminder.ReplyAuthorId}>";
                            builder.WithContent(message);
                        }
                    }
                    else
                    {
                        bool validMessage;
                        
                        try { validMessage = await channel.GetMessageAsync(reminder.MessageId) is not null; }
                        catch (NotFoundException) { validMessage = false; }
                        if (validMessage)
                        {
                            builder.WithReply(reminder.MessageId, true);
                            builder.WithContent("You wanted me to remind you of this!");
                        }
                        else
                        {
                            message += "\n(Your message was deleted, hence the lack of a reply!)";
                            builder.WithContent(message);
                        }
                    }
                    await builder.SendAsync(channel);
                }
                _logger.LogTrace("Succesfully sent reminder. Removing from database");
                
                using IServiceScope scope = _services.CreateScope();
                var mediator = _services.CreateScope().ServiceProvider.Get<IMediator>();
                await mediator.Send(new ReminderRequest.Remove(reminder.Id));
            }
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Started!");
            
            using IServiceScope scope = _services.CreateScope();
            var mediator = _services.CreateScope().ServiceProvider.Get<IMediator>();
            _reminders = (await mediator.Send(new ReminderRequest.GetAll(), stoppingToken)).ToList();
            _logger.LogTrace("Acquired reminders.");
            _logger.LogDebug("Starting reminder callback timer");
            _timer = new(Tick, DateTime.Now, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            
            await Task.Delay(-1, stoppingToken);
        }
    }
}