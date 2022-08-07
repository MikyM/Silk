﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Remora.Rest.Core;
using Silk.Data.Entities;

namespace Silk.Data.MediatR.Reminders;

public static class CreateReminder
{
    /// <summary>
    /// Request for creating a <see cref="ReminderEntity" />.
    /// </summary>
    public sealed record Request
        (
            DateTimeOffset Expiration,
            Snowflake      OwnerID,
            Snowflake      ChannelID,
            Snowflake?     MessageID,
            Snowflake?     GuildID,
            string?        MessageContent,
            Snowflake?     ReplyID             = null,
            Snowflake?     ReplyAuthorID       = null,
            string?        ReplyMessageContent = null
        ) : IRequest<ReminderEntity>;

    /// <summary>
    /// The default handler for <see cref="T:Silk.Data.MediatR.Reminders.Request" />.
    /// </summary>
    internal sealed class Handler : IRequestHandler<Request, ReminderEntity>
    {
        private readonly GuildContext _db;

        public Handler(GuildContext db) => _db = db;

        public async Task<ReminderEntity> Handle(Request request, CancellationToken cancellationToken)
        {
            var reminder = new ReminderEntity
            {
                ExpiresAt           = request.Expiration,
                CreatedAt           = DateTimeOffset.UtcNow,
                OwnerID             = request.OwnerID,
                ChannelID           = request.ChannelID,
                MessageID           = request.MessageID,
                GuildID             = request.GuildID,
                ReplyMessageID      = request.ReplyID,
                MessageContent      = request.MessageContent,
                ReplyAuthorID       = request.ReplyAuthorID,
                ReplyMessageContent = request.ReplyMessageContent,
                IsPrivate           = request.MessageID is null || request.GuildID is null,
                IsReply             = request.ReplyID is not null,
            };

            _db.Add(reminder);

            await _db.SaveChangesAsync(cancellationToken);

            return reminder;
        }
    }
}