﻿using System;

namespace Silk.Core.Data.Entities
{
    public class InfractionEntity
    {
        public int Id { get; set; } //Requisite Id for DB purposes
        /// <summary>
        /// The Id of the user this infraction belongs to.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// The Id of the guild this infraction was given on.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// The guild-specific case Id of this infraction.
        /// </summary>
        public int CaseNumber { get; set; }

        /// <summary>
        /// The guild this infraction was given on.
        /// </summary>
        public GuildEntity Guild { get; set; }
        
        /// <summary>
        /// The User this infraction was given to. 
        /// </summary>
        public UserEntity User { get; set; }

        /// <summary>
        /// The reason this infraction was given. Infractions added by Auto-Mod will be prefixed with "[AUTO-MOD]".
        /// </summary>
        public string Reason { get; set; } = "Not given."; // Why was this infraction given

        /// <summary>
        /// Whether this infraction has been processed.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// The Id of the user that gave this infraction; Auto-Mod infractions will default to the bot.
        /// </summary>
        public ulong Enforcer { get; set; } //Who gave this infraction
        
        /// <summary>
        /// The time this infraction was added.
        /// </summary>
        public DateTime InfractionTime { get; set; } //When it happened

        /// <summary>
        /// When this infraction was last changed.
        /// </summary>
        public DateTime? LastUpdated { get; set; }
        
        /// <summary>
        /// The type of infraction.
        /// </summary>
        public InfractionType InfractionType { get; set; } //What happened
        
        /// <summary>
        /// Whether this was initially intended to be a strike.
        /// </summary>
        public bool EscalatedFromStrike { get; set; }

        /// <summary>
        /// Whether this is an active infraction and/or this infraction counts toward any auto-incrementing severity of infractions.
        /// Infraction will still hold on the user's record but is not held against them if set to false.
        /// </summary>
        public bool HeldAgainstUser { get; set; } = true; // Used for infraction service to determine whether to escalate or not //

        /// <summary>
        /// When this infraction is set to expire. Resolves to null
        /// </summary>
        public DateTime? Expiration { get; set; }
    }
}