﻿using System;

namespace Silk.Core.Data.Entities
{
    public class ReminderEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// When this reminder expires.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// When this reminder was created.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// The Id of the owner.
        /// </summary>
        public ulong OwnerId { get; init; }

        /// <summary>
        /// The channel the reminder was made in
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// The guild this reminder was created in.
        /// </summary>
        public ulong? GuildId { get; set; }

        /// <summary>
        /// The Id of the message to remind them of
        /// </summary>
        public ulong MessageId { get; set; }

        /// <summary>
        /// The type of reminder, be it recurring or non-recurring.
        /// </summary>
        public ReminderType Type { get; set; }

        /// <summary>
        /// The content of the original reminder, in case a message can't be sent to the original channel.
        /// </summary>
        public string? MessageContent { get; set; }

        /// <summary>
        /// The content of the message the reply contained, if the reminder was a reply.
        /// </summary>
        public string? ReplyMessageContent { get; set; }

        /// <summary>
        /// The Id of the author of the reply the reminder was set with, if any.
        /// </summary>
        public ulong? ReplyAuthorId { get; set; }

        /// <summary>
        /// The Id of the message that was replied to, if any.
        /// </summary>
        public ulong? ReplyId { get; set; }

        /// <summary>
        /// Whether or not the reminder was replying to a different message.
        /// </summary>
        public bool WasReply { get; set; }


        public ReminderEntity() { }
        public ReminderEntity(int id, DateTime expiration, DateTime creationTime, ulong ownerId, ulong channelId, ulong guildId, ulong messageId, ReminderType type, string? messageContent, string? replyMessageContent, ulong? replyAuthorId, ulong? replyId, bool wasReply)
        {
            Id = id;
            Expiration = expiration;
            CreationTime = creationTime;
            OwnerId = ownerId;
            ChannelId = channelId;
            GuildId = guildId;
            MessageId = messageId;
            Type = type;
            MessageContent = messageContent;
            ReplyMessageContent = replyMessageContent;
            ReplyAuthorId = replyAuthorId;
            ReplyId = replyId;
            WasReply = wasReply;
        }
    }

}