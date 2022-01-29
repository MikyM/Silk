﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Silk.Data.Migrations
{
    public partial class MigrationOrSomething : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExemptionEntity_GuildModConfigs_GuildModConfigEntityId",
                table: "ExemptionEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_GuildConfigs_Guilds_GuildId",
                table: "GuildConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_GuildModConfigs_Guilds_GuildId",
                table: "GuildModConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_Infractions_Guilds_GuildId",
                table: "Infractions");

            migrationBuilder.DropForeignKey(
                name: "FK_Infractions_Users_UserId_GuildId",
                table: "Infractions");

            migrationBuilder.DropForeignKey(
                name: "FK_InfractionStepEntity_GuildModConfigs_ConfigId",
                table: "InfractionStepEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_InviteEntity_GuildModConfigs_GuildModConfigEntityId",
                table: "InviteEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Guilds_GuildEntityId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Tags_OriginalTagId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_UserHistoryEntity_Users_UserId_GuildId",
                table: "UserHistoryEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Guilds_GuildId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "DisabledCommandEntity");

            migrationBuilder.DropTable(
                name: "GlobalUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Infractions",
                table: "Infractions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserHistoryEntity",
                table: "UserHistoryEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InviteEntity",
                table: "InviteEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InfractionStepEntity",
                table: "InfractionStepEntity");

            migrationBuilder.DropIndex(
                name: "IX_InfractionStepEntity_ConfigId",
                table: "InfractionStepEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildModConfigs",
                table: "GuildModConfigs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildConfigs",
                table: "GuildConfigs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExemptionEntity",
                table: "ExemptionEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommandInvocations",
                table: "CommandInvocations");

            migrationBuilder.DropColumn(
                name: "DatabaseId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InitialJoinDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "Infractions");

            migrationBuilder.DropColumn(
                name: "InfractionTime",
                table: "Infractions");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Infractions");

            migrationBuilder.DropColumn(
                name: "AutoDehoist",
                table: "GuildModConfigs");

            migrationBuilder.DropColumn(
                name: "AutoEscalateInfractions",
                table: "GuildModConfigs");

            migrationBuilder.DropColumn(
                name: "BlacklistInvites",
                table: "GuildModConfigs");

            migrationBuilder.DropColumn(
                name: "BlacklistWords",
                table: "GuildModConfigs");

            migrationBuilder.DropColumn(
                name: "LogMemberJoins",
                table: "GuildModConfigs");

            migrationBuilder.DropColumn(
                name: "LogMemberLeaves",
                table: "GuildModConfigs");

            migrationBuilder.DropColumn(
                name: "LoggingChannel",
                table: "GuildModConfigs");

            migrationBuilder.DropColumn(
                name: "LoggingWebhookUrl",
                table: "GuildModConfigs");

            migrationBuilder.DropColumn(
                name: "WebhookLoggingId",
                table: "GuildModConfigs");

            migrationBuilder.DropColumn(
                name: "GreetingChannel",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "GreetingOption",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "GreetingText",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "VerificationRole",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "CommandInvocations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CommandInvocations");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Reminders",
                newName: "reminders");

            migrationBuilder.RenameTable(
                name: "Infractions",
                newName: "infractions");

            migrationBuilder.RenameTable(
                name: "Guilds",
                newName: "guilds");

            migrationBuilder.RenameTable(
                name: "UserHistoryEntity",
                newName: "user_histories");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "guild_tags");

            migrationBuilder.RenameTable(
                name: "InviteEntity",
                newName: "invites");

            migrationBuilder.RenameTable(
                name: "InfractionStepEntity",
                newName: "infraction_steps");

            migrationBuilder.RenameTable(
                name: "GuildModConfigs",
                newName: "guild_moderation_config");

            migrationBuilder.RenameTable(
                name: "GuildConfigs",
                newName: "guild_configs");

            migrationBuilder.RenameTable(
                name: "ExemptionEntity",
                newName: "infraction_exemptions");

            migrationBuilder.RenameTable(
                name: "CommandInvocations",
                newName: "command_invocations");

            migrationBuilder.RenameColumn(
                name: "Flags",
                table: "users",
                newName: "flags");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "users",
                newName: "guild_id");

            migrationBuilder.RenameIndex(
                name: "IX_Users_GuildId",
                table: "users",
                newName: "IX_users_guild_id");

            migrationBuilder.RenameColumn(
                name: "ReplyMessageContent",
                table: "reminders",
                newName: "reply_content");

            migrationBuilder.RenameColumn(
                name: "ReplyAuthorId",
                table: "reminders",
                newName: "reply_author_id");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "reminders",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "reminders",
                newName: "message_id");

            migrationBuilder.RenameColumn(
                name: "MessageContent",
                table: "reminders",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "reminders",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "reminders",
                newName: "channel_id");

            migrationBuilder.RenameColumn(
                name: "WasReply",
                table: "reminders",
                newName: "is_reply");

            migrationBuilder.RenameColumn(
                name: "ReplyId",
                table: "reminders",
                newName: "reply_message_id");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "infractions",
                newName: "reason");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "infractions",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "CaseNumber",
                table: "infractions",
                newName: "case_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "infractions",
                newName: "target_id");

            migrationBuilder.RenameColumn(
                name: "InfractionType",
                table: "infractions",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "HeldAgainstUser",
                table: "infractions",
                newName: "user_notified");

            migrationBuilder.RenameColumn(
                name: "Handled",
                table: "infractions",
                newName: "processed");

            migrationBuilder.RenameColumn(
                name: "EscalatedFromStrike",
                table: "infractions",
                newName: "escalated");

            migrationBuilder.RenameColumn(
                name: "Enforcer",
                table: "infractions",
                newName: "enforcer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Infractions_UserId_GuildId",
                table: "infractions",
                newName: "IX_infractions_target_id_guild_id");

            migrationBuilder.RenameIndex(
                name: "IX_Infractions_GuildId",
                table: "infractions",
                newName: "IX_infractions_guild_id");

            migrationBuilder.RenameColumn(
                name: "Prefix",
                table: "guilds",
                newName: "prefix");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_histories",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "LeaveDates",
                table: "user_histories",
                newName: "leave_dates");

            migrationBuilder.RenameColumn(
                name: "JoinDates",
                table: "user_histories",
                newName: "join_dates");

            migrationBuilder.RenameColumn(
                name: "JoinDate",
                table: "user_histories",
                newName: "initial_join_date");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "user_histories",
                newName: "guild_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserHistoryEntity_UserId_GuildId",
                table: "user_histories",
                newName: "IX_user_histories_user_id_guild_id");

            migrationBuilder.RenameColumn(
                name: "Uses",
                table: "guild_tags",
                newName: "uses");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "guild_tags",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "GuildEntityId",
                table: "guild_tags",
                newName: "GuildEntityID");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "guild_tags",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "guild_tags",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "OriginalTagId",
                table: "guild_tags",
                newName: "parent_id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "guild_tags",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "guild_tags",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_OriginalTagId",
                table: "guild_tags",
                newName: "IX_guild_tags_parent_id");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_GuildEntityId",
                table: "guild_tags",
                newName: "IX_guild_tags_GuildEntityID");

            migrationBuilder.RenameColumn(
                name: "VanityURL",
                table: "invites",
                newName: "invite_code");

            migrationBuilder.RenameColumn(
                name: "InviteGuildId",
                table: "invites",
                newName: "invite_guild_id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "invites",
                newName: "guild_id");

            migrationBuilder.RenameIndex(
                name: "IX_InviteEntity_GuildModConfigEntityId",
                table: "invites",
                newName: "IX_invites_GuildModConfigEntityId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "infraction_steps",
                newName: "infraction_type");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "infraction_steps",
                newName: "infraction_duration");

            migrationBuilder.RenameColumn(
                name: "ConfigId",
                table: "infraction_steps",
                newName: "config_id");

            migrationBuilder.RenameColumn(
                name: "UseAggressiveRegex",
                table: "guild_moderation_config",
                newName: "match_aggressively");

            migrationBuilder.RenameColumn(
                name: "MuteRoleId",
                table: "guild_moderation_config",
                newName: "mute_role");

            migrationBuilder.RenameColumn(
                name: "MaxUserMentions",
                table: "guild_moderation_config",
                newName: "max_user_mentions");

            migrationBuilder.RenameColumn(
                name: "MaxRoleMentions",
                table: "guild_moderation_config",
                newName: "max_role_mentions");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "guild_moderation_config",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "DetectPhishingLinks",
                table: "guild_moderation_config",
                newName: "detect_phishing");

            migrationBuilder.RenameColumn(
                name: "DeletePhishingLinks",
                table: "guild_moderation_config",
                newName: "delete_detected_phishing");

            migrationBuilder.RenameColumn(
                name: "DeleteMessageOnMatchedInvite",
                table: "guild_moderation_config",
                newName: "delete_invite_messages");

            migrationBuilder.RenameColumn(
                name: "WarnOnMatchedInvite",
                table: "guild_moderation_config",
                newName: "scan_invite_origin");

            migrationBuilder.RenameColumn(
                name: "UseWebhookLogging",
                table: "guild_moderation_config",
                newName: "progressive_infractions");

            migrationBuilder.RenameColumn(
                name: "ScanInvites",
                table: "guild_moderation_config",
                newName: "invite_whitelist_enabled");

            migrationBuilder.RenameColumn(
                name: "LogMessageChanges",
                table: "guild_moderation_config",
                newName: "infract_on_invite");

            migrationBuilder.RenameIndex(
                name: "IX_GuildModConfigs_GuildId",
                table: "guild_moderation_config",
                newName: "IX_guild_moderation_config_guild_id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "guild_configs",
                newName: "guild_id");

            migrationBuilder.RenameIndex(
                name: "IX_GuildConfigs_GuildId",
                table: "guild_configs",
                newName: "IX_guild_configs_guild_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExemptionEntity_GuildModConfigEntityId",
                table: "infraction_exemptions",
                newName: "IX_infraction_exemptions_GuildModConfigEntityId");

            migrationBuilder.RenameColumn(
                name: "CommandName",
                table: "command_invocations",
                newName: "command_name");

            migrationBuilder.AlterColumn<ulong>(
                name: "message_id",
                table: "reminders",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "reminders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "expires_at",
                table: "reminders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "is_private",
                table: "reminders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active",
                table: "infractions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "infractions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "expires_at",
                table: "infractions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<List<DateTimeOffset>>(
                name: "leave_dates",
                table: "user_histories",
                type: "timestamp with time zone[]",
                nullable: false,
                oldClrType: typeof(List<DateTime>),
                oldType: "timestamp without time zone[]");

            migrationBuilder.AlterColumn<List<DateTimeOffset>>(
                name: "join_dates",
                table: "user_histories",
                type: "timestamp with time zone[]",
                nullable: false,
                oldClrType: typeof(List<DateTime>),
                oldType: "timestamp without time zone[]");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "initial_join_date",
                table: "user_histories",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "guild_tags",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "GuildModConfigEntityId",
                table: "infraction_steps",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "infraction_count",
                table: "infraction_steps",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoggingConfigId",
                table: "guild_moderation_config",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"ALTER TABLE infraction_exemptions ALTER COLUMN exempt_from TYPE INTEGER USING (exempt_from::integer)");
            
            migrationBuilder.AddColumn<DateTime>(
                name: "used_at",
                table: "command_invocations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                columns: new[] { "id", "guild_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_reminders",
                table: "reminders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_infractions",
                table: "infractions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_guilds",
                table: "guilds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_histories",
                table: "user_histories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_guild_tags",
                table: "guild_tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invites",
                table: "invites",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_infraction_steps",
                table: "infraction_steps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_guild_moderation_config",
                table: "guild_moderation_config",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_guild_configs",
                table: "guild_configs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_infraction_exemptions",
                table: "infraction_exemptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_command_invocations",
                table: "command_invocations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "guild_greetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildID = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Option = table.Column<int>(type: "integer", nullable: false),
                    ChannelID = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    MetadataID = table.Column<ulong>(type: "numeric(20,0)", nullable: true),
                    GuildConfigEntityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guild_greetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guild_greetings_guild_configs_GuildConfigEntityId",
                        column: x => x.GuildConfigEntityId,
                        principalTable: "guild_configs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "logging_channels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    webhook_id = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    webhook_token = table.Column<string>(type: "text", nullable: false),
                    channel_id = table.Column<ulong>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logging_channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "guild_logging_configs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    log_message_edits = table.Column<bool>(type: "boolean", nullable: false),
                    log_message_deletes = table.Column<bool>(type: "boolean", nullable: false),
                    log_infractions = table.Column<bool>(type: "boolean", nullable: false),
                    log_member_joins = table.Column<bool>(type: "boolean", nullable: false),
                    log_member_leaves = table.Column<bool>(type: "boolean", nullable: false),
                    fallback_logging_channel = table.Column<ulong>(type: "numeric(20,0)", nullable: true),
                    use_webhook_logging = table.Column<bool>(type: "boolean", nullable: false),
                    InfractionsId = table.Column<int>(type: "integer", nullable: true),
                    MessageEditsId = table.Column<int>(type: "integer", nullable: true),
                    MessageDeletesId = table.Column<int>(type: "integer", nullable: true),
                    MemberJoinsId = table.Column<int>(type: "integer", nullable: true),
                    MemberLeavesId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guild_logging_configs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guild_logging_configs_logging_channels_InfractionsId",
                        column: x => x.InfractionsId,
                        principalTable: "logging_channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_guild_logging_configs_logging_channels_MemberJoinsId",
                        column: x => x.MemberJoinsId,
                        principalTable: "logging_channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_guild_logging_configs_logging_channels_MemberLeavesId",
                        column: x => x.MemberLeavesId,
                        principalTable: "logging_channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_guild_logging_configs_logging_channels_MessageDeletesId",
                        column: x => x.MessageDeletesId,
                        principalTable: "logging_channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_guild_logging_configs_logging_channels_MessageEditsId",
                        column: x => x.MessageEditsId,
                        principalTable: "logging_channels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_infraction_steps_GuildModConfigEntityId",
                table: "infraction_steps",
                column: "GuildModConfigEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_guild_moderation_config_LoggingConfigId",
                table: "guild_moderation_config",
                column: "LoggingConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_guild_greetings_GuildConfigEntityId",
                table: "guild_greetings",
                column: "GuildConfigEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_guild_logging_configs_InfractionsId",
                table: "guild_logging_configs",
                column: "InfractionsId");

            migrationBuilder.CreateIndex(
                name: "IX_guild_logging_configs_MemberJoinsId",
                table: "guild_logging_configs",
                column: "MemberJoinsId");

            migrationBuilder.CreateIndex(
                name: "IX_guild_logging_configs_MemberLeavesId",
                table: "guild_logging_configs",
                column: "MemberLeavesId");

            migrationBuilder.CreateIndex(
                name: "IX_guild_logging_configs_MessageDeletesId",
                table: "guild_logging_configs",
                column: "MessageDeletesId");

            migrationBuilder.CreateIndex(
                name: "IX_guild_logging_configs_MessageEditsId",
                table: "guild_logging_configs",
                column: "MessageEditsId");

            migrationBuilder.AddForeignKey(
                name: "FK_guild_configs_guilds_guild_id",
                table: "guild_configs",
                column: "guild_id",
                principalTable: "guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_guild_moderation_config_guild_logging_configs_LoggingConfig~",
                table: "guild_moderation_config",
                column: "LoggingConfigId",
                principalTable: "guild_logging_configs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_guild_moderation_config_guilds_guild_id",
                table: "guild_moderation_config",
                column: "guild_id",
                principalTable: "guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_guild_tags_guild_tags_parent_id",
                table: "guild_tags",
                column: "parent_id",
                principalTable: "guild_tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_guild_tags_guilds_GuildEntityID",
                table: "guild_tags",
                column: "GuildEntityID",
                principalTable: "guilds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_infraction_exemptions_guild_moderation_config_GuildModConfi~",
                table: "infraction_exemptions",
                column: "GuildModConfigEntityId",
                principalTable: "guild_moderation_config",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_infraction_steps_guild_moderation_config_GuildModConfigEnti~",
                table: "infraction_steps",
                column: "GuildModConfigEntityId",
                principalTable: "guild_moderation_config",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_infractions_guilds_guild_id",
                table: "infractions",
                column: "guild_id",
                principalTable: "guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_infractions_users_target_id_guild_id",
                table: "infractions",
                columns: new[] { "target_id", "guild_id" },
                principalTable: "users",
                principalColumns: new[] { "id", "guild_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_invites_guild_moderation_config_GuildModConfigEntityId",
                table: "invites",
                column: "GuildModConfigEntityId",
                principalTable: "guild_moderation_config",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_histories_users_user_id_guild_id",
                table: "user_histories",
                columns: new[] { "user_id", "guild_id" },
                principalTable: "users",
                principalColumns: new[] { "id", "guild_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_guilds_guild_id",
                table: "users",
                column: "guild_id",
                principalTable: "guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_guild_configs_guilds_guild_id",
                table: "guild_configs");

            migrationBuilder.DropForeignKey(
                name: "FK_guild_moderation_config_guild_logging_configs_LoggingConfig~",
                table: "guild_moderation_config");

            migrationBuilder.DropForeignKey(
                name: "FK_guild_moderation_config_guilds_guild_id",
                table: "guild_moderation_config");

            migrationBuilder.DropForeignKey(
                name: "FK_guild_tags_guild_tags_parent_id",
                table: "guild_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_guild_tags_guilds_GuildEntityID",
                table: "guild_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_infraction_exemptions_guild_moderation_config_GuildModConfi~",
                table: "infraction_exemptions");

            migrationBuilder.DropForeignKey(
                name: "FK_infraction_steps_guild_moderation_config_GuildModConfigEnti~",
                table: "infraction_steps");

            migrationBuilder.DropForeignKey(
                name: "FK_infractions_guilds_guild_id",
                table: "infractions");

            migrationBuilder.DropForeignKey(
                name: "FK_infractions_users_target_id_guild_id",
                table: "infractions");

            migrationBuilder.DropForeignKey(
                name: "FK_invites_guild_moderation_config_GuildModConfigEntityId",
                table: "invites");

            migrationBuilder.DropForeignKey(
                name: "FK_user_histories_users_user_id_guild_id",
                table: "user_histories");

            migrationBuilder.DropForeignKey(
                name: "FK_users_guilds_guild_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "guild_greetings");

            migrationBuilder.DropTable(
                name: "guild_logging_configs");

            migrationBuilder.DropTable(
                name: "logging_channels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reminders",
                table: "reminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_infractions",
                table: "infractions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_guilds",
                table: "guilds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_histories",
                table: "user_histories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invites",
                table: "invites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_infraction_steps",
                table: "infraction_steps");

            migrationBuilder.DropIndex(
                name: "IX_infraction_steps_GuildModConfigEntityId",
                table: "infraction_steps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_infraction_exemptions",
                table: "infraction_exemptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_guild_tags",
                table: "guild_tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_guild_moderation_config",
                table: "guild_moderation_config");

            migrationBuilder.DropIndex(
                name: "IX_guild_moderation_config_LoggingConfigId",
                table: "guild_moderation_config");

            migrationBuilder.DropPrimaryKey(
                name: "PK_guild_configs",
                table: "guild_configs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_command_invocations",
                table: "command_invocations");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "reminders");

            migrationBuilder.DropColumn(
                name: "expires_at",
                table: "reminders");

            migrationBuilder.DropColumn(
                name: "is_private",
                table: "reminders");

            migrationBuilder.DropColumn(
                name: "active",
                table: "infractions");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "infractions");

            migrationBuilder.DropColumn(
                name: "expires_at",
                table: "infractions");

            migrationBuilder.DropColumn(
                name: "GuildModConfigEntityId",
                table: "infraction_steps");

            migrationBuilder.DropColumn(
                name: "infraction_count",
                table: "infraction_steps");

            migrationBuilder.DropColumn(
                name: "LoggingConfigId",
                table: "guild_moderation_config");

            migrationBuilder.DropColumn(
                name: "used_at",
                table: "command_invocations");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "reminders",
                newName: "Reminders");

            migrationBuilder.RenameTable(
                name: "infractions",
                newName: "Infractions");

            migrationBuilder.RenameTable(
                name: "guilds",
                newName: "Guilds");

            migrationBuilder.RenameTable(
                name: "user_histories",
                newName: "UserHistoryEntity");

            migrationBuilder.RenameTable(
                name: "invites",
                newName: "InviteEntity");

            migrationBuilder.RenameTable(
                name: "infraction_steps",
                newName: "InfractionStepEntity");

            migrationBuilder.RenameTable(
                name: "infraction_exemptions",
                newName: "ExemptionEntity");

            migrationBuilder.RenameTable(
                name: "guild_tags",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "guild_moderation_config",
                newName: "GuildModConfigs");

            migrationBuilder.RenameTable(
                name: "guild_configs",
                newName: "GuildConfigs");

            migrationBuilder.RenameTable(
                name: "command_invocations",
                newName: "CommandInvocations");

            migrationBuilder.RenameColumn(
                name: "flags",
                table: "Users",
                newName: "Flags");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Users",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_users_guild_id",
                table: "Users",
                newName: "IX_Users_GuildId");

            migrationBuilder.RenameColumn(
                name: "reply_content",
                table: "Reminders",
                newName: "ReplyMessageContent");

            migrationBuilder.RenameColumn(
                name: "reply_author_id",
                table: "Reminders",
                newName: "ReplyAuthorId");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "Reminders",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "message_id",
                table: "Reminders",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Reminders",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Reminders",
                newName: "MessageContent");

            migrationBuilder.RenameColumn(
                name: "channel_id",
                table: "Reminders",
                newName: "ChannelId");

            migrationBuilder.RenameColumn(
                name: "reply_message_id",
                table: "Reminders",
                newName: "ReplyId");

            migrationBuilder.RenameColumn(
                name: "is_reply",
                table: "Reminders",
                newName: "WasReply");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "Infractions",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Infractions",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "case_id",
                table: "Infractions",
                newName: "CaseNumber");

            migrationBuilder.RenameColumn(
                name: "user_notified",
                table: "Infractions",
                newName: "HeldAgainstUser");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Infractions",
                newName: "InfractionType");

            migrationBuilder.RenameColumn(
                name: "target_id",
                table: "Infractions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "processed",
                table: "Infractions",
                newName: "Handled");

            migrationBuilder.RenameColumn(
                name: "escalated",
                table: "Infractions",
                newName: "EscalatedFromStrike");

            migrationBuilder.RenameColumn(
                name: "enforcer_id",
                table: "Infractions",
                newName: "Enforcer");

            migrationBuilder.RenameIndex(
                name: "IX_infractions_target_id_guild_id",
                table: "Infractions",
                newName: "IX_Infractions_UserId_GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_infractions_guild_id",
                table: "Infractions",
                newName: "IX_Infractions_GuildId");

            migrationBuilder.RenameColumn(
                name: "prefix",
                table: "Guilds",
                newName: "Prefix");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserHistoryEntity",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "leave_dates",
                table: "UserHistoryEntity",
                newName: "LeaveDates");

            migrationBuilder.RenameColumn(
                name: "join_dates",
                table: "UserHistoryEntity",
                newName: "JoinDates");

            migrationBuilder.RenameColumn(
                name: "initial_join_date",
                table: "UserHistoryEntity",
                newName: "JoinDate");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "UserHistoryEntity",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_user_histories_user_id_guild_id",
                table: "UserHistoryEntity",
                newName: "IX_UserHistoryEntity_UserId_GuildId");

            migrationBuilder.RenameColumn(
                name: "invite_guild_id",
                table: "InviteEntity",
                newName: "InviteGuildId");

            migrationBuilder.RenameColumn(
                name: "invite_code",
                table: "InviteEntity",
                newName: "VanityURL");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "InviteEntity",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_invites_GuildModConfigEntityId",
                table: "InviteEntity",
                newName: "IX_InviteEntity_GuildModConfigEntityId");

            migrationBuilder.RenameColumn(
                name: "infraction_type",
                table: "InfractionStepEntity",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "infraction_duration",
                table: "InfractionStepEntity",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "config_id",
                table: "InfractionStepEntity",
                newName: "ConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_infraction_exemptions_GuildModConfigEntityId",
                table: "ExemptionEntity",
                newName: "IX_ExemptionEntity_GuildModConfigEntityId");

            migrationBuilder.RenameColumn(
                name: "uses",
                table: "Tags",
                newName: "Uses");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Tags",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Tags",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "GuildEntityID",
                table: "Tags",
                newName: "GuildEntityId");

            migrationBuilder.RenameColumn(
                name: "parent_id",
                table: "Tags",
                newName: "OriginalTagId");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "Tags",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Tags",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Tags",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_guild_tags_parent_id",
                table: "Tags",
                newName: "IX_Tags_OriginalTagId");

            migrationBuilder.RenameIndex(
                name: "IX_guild_tags_GuildEntityID",
                table: "Tags",
                newName: "IX_Tags_GuildEntityId");

            migrationBuilder.RenameColumn(
                name: "mute_role",
                table: "GuildModConfigs",
                newName: "MuteRoleId");

            migrationBuilder.RenameColumn(
                name: "max_user_mentions",
                table: "GuildModConfigs",
                newName: "MaxUserMentions");

            migrationBuilder.RenameColumn(
                name: "max_role_mentions",
                table: "GuildModConfigs",
                newName: "MaxRoleMentions");

            migrationBuilder.RenameColumn(
                name: "match_aggressively",
                table: "GuildModConfigs",
                newName: "UseAggressiveRegex");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "GuildModConfigs",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "detect_phishing",
                table: "GuildModConfigs",
                newName: "DetectPhishingLinks");

            migrationBuilder.RenameColumn(
                name: "delete_invite_messages",
                table: "GuildModConfigs",
                newName: "DeleteMessageOnMatchedInvite");

            migrationBuilder.RenameColumn(
                name: "delete_detected_phishing",
                table: "GuildModConfigs",
                newName: "DeletePhishingLinks");

            migrationBuilder.RenameColumn(
                name: "scan_invite_origin",
                table: "GuildModConfigs",
                newName: "WarnOnMatchedInvite");

            migrationBuilder.RenameColumn(
                name: "progressive_infractions",
                table: "GuildModConfigs",
                newName: "UseWebhookLogging");

            migrationBuilder.RenameColumn(
                name: "invite_whitelist_enabled",
                table: "GuildModConfigs",
                newName: "ScanInvites");

            migrationBuilder.RenameColumn(
                name: "infract_on_invite",
                table: "GuildModConfigs",
                newName: "LogMessageChanges");

            migrationBuilder.RenameIndex(
                name: "IX_guild_moderation_config_guild_id",
                table: "GuildModConfigs",
                newName: "IX_GuildModConfigs_GuildId");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "GuildConfigs",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_guild_configs_guild_id",
                table: "GuildConfigs",
                newName: "IX_GuildConfigs_GuildId");

            migrationBuilder.RenameColumn(
                name: "command_name",
                table: "CommandInvocations",
                newName: "CommandName");

            migrationBuilder.AddColumn<long>(
                name: "DatabaseId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "InitialJoinDate",
                table: "Users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "MessageId",
                table: "Reminders",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(ulong),
                oldType: "numeric(20,0)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Reminders",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "Reminders",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Reminders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "Infractions",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InfractionTime",
                table: "Infractions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Infractions",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AlterColumn<List<DateTime>>(
                name: "LeaveDates",
                table: "UserHistoryEntity",
                type: "timestamp without time zone[]",
                nullable: false,
                oldClrType: typeof(List<DateTimeOffset>),
                oldType: "timestamp with time zone[]");

            migrationBuilder.AlterColumn<List<DateTime>>(
                name: "JoinDates",
                table: "UserHistoryEntity",
                type: "timestamp without time zone[]",
                nullable: false,
                oldClrType: typeof(List<DateTimeOffset>),
                oldType: "timestamp with time zone[]");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinDate",
                table: "UserHistoryEntity",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "exempt_from",
                table: "ExemptionEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Tags",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "AutoDehoist",
                table: "GuildModConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoEscalateInfractions",
                table: "GuildModConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlacklistInvites",
                table: "GuildModConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlacklistWords",
                table: "GuildModConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LogMemberJoins",
                table: "GuildModConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LogMemberLeaves",
                table: "GuildModConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LoggingChannel",
                table: "GuildModConfigs",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LoggingWebhookUrl",
                table: "GuildModConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WebhookLoggingId",
                table: "GuildModConfigs",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GreetingChannel",
                table: "GuildConfigs",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "GreetingOption",
                table: "GuildConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GreetingText",
                table: "GuildConfigs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "VerificationRole",
                table: "GuildConfigs",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GuildId",
                table: "CommandInvocations",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UserId",
                table: "CommandInvocations",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                columns: new[] { "Id", "GuildId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Infractions",
                table: "Infractions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserHistoryEntity",
                table: "UserHistoryEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InviteEntity",
                table: "InviteEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InfractionStepEntity",
                table: "InfractionStepEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExemptionEntity",
                table: "ExemptionEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildModConfigs",
                table: "GuildModConfigs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildConfigs",
                table: "GuildConfigs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommandInvocations",
                table: "CommandInvocations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DisabledCommandEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CommandName = table.Column<string>(type: "text", nullable: false),
                    GuildConfigEntityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisabledCommandEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisabledCommandEntity_GuildConfigs_GuildConfigEntityId",
                        column: x => x.GuildConfigEntityId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DisabledCommandEntity_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlobalUsers",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Cash = table.Column<int>(type: "integer", nullable: false),
                    LastCashOut = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfractionStepEntity_ConfigId",
                table: "InfractionStepEntity",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_DisabledCommandEntity_GuildConfigEntityId",
                table: "DisabledCommandEntity",
                column: "GuildConfigEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DisabledCommandEntity_GuildId_CommandName",
                table: "DisabledCommandEntity",
                columns: new[] { "GuildId", "CommandName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExemptionEntity_GuildModConfigs_GuildModConfigEntityId",
                table: "ExemptionEntity",
                column: "GuildModConfigEntityId",
                principalTable: "GuildModConfigs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildConfigs_Guilds_GuildId",
                table: "GuildConfigs",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuildModConfigs_Guilds_GuildId",
                table: "GuildModConfigs",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Infractions_Guilds_GuildId",
                table: "Infractions",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Infractions_Users_UserId_GuildId",
                table: "Infractions",
                columns: new[] { "UserId", "GuildId" },
                principalTable: "Users",
                principalColumns: new[] { "Id", "GuildId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InfractionStepEntity_GuildModConfigs_ConfigId",
                table: "InfractionStepEntity",
                column: "ConfigId",
                principalTable: "GuildModConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InviteEntity_GuildModConfigs_GuildModConfigEntityId",
                table: "InviteEntity",
                column: "GuildModConfigEntityId",
                principalTable: "GuildModConfigs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Guilds_GuildEntityId",
                table: "Tags",
                column: "GuildEntityId",
                principalTable: "Guilds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Tags_OriginalTagId",
                table: "Tags",
                column: "OriginalTagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserHistoryEntity_Users_UserId_GuildId",
                table: "UserHistoryEntity",
                columns: new[] { "UserId", "GuildId" },
                principalTable: "Users",
                principalColumns: new[] { "Id", "GuildId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Guilds_GuildId",
                table: "Users",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
