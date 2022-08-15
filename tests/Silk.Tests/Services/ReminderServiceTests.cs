using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;
using Remora.Results;
using Silk.Data.Entities;
using Silk.Data.MediatR.Reminders;
using Silk.Services.Guild;

namespace Silk.Tests.Services;

public class ReminderServiceTests
{
    [Test]
    public async Task PullsActiveRemindersOnStartup()
    {
        // Arrange
        var mediatorMock    = new Mock<IMediator>();
        var reminderService = new ReminderService(mediatorMock.Object, new ShardIdentification(0, 1), Mock.Of<IDiscordRestUserAPI>(), Mock.Of<IDiscordRestChannelAPI>(), NullLogger<ReminderService>.Instance);

        // Act
        await reminderService.StartAsync(default);
        await reminderService.StopAsync(default);

        // Assert
        mediatorMock.Verify(x => x.Send(It.IsAny<GetAllReminders.Request>(), default), Times.Once);
    }


    [Test]
    public async Task CreatesReminderSuccessfully()
    {
        // Arrange
        var mediatorMock    = new Mock<IMediator>();
        
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateReminder.Request>(), default))
                    .ReturnsAsync(new ReminderEntity());
        
        var reminderService = new ReminderService(mediatorMock.Object, new ShardIdentification(0, 1), Mock.Of<IDiscordRestUserAPI>(), Mock.Of<IDiscordRestChannelAPI>(), NullLogger<ReminderService>.Instance);
        // Act
        await reminderService.CreateReminderAsync(default, default, default, default, default, default);

        // Assert
        mediatorMock.Verify(x => x.Send(It.IsAny<CreateReminder.Request>(), default), Times.Once);
    }

    [Test]
    public async Task CreatesDMMessageWhenMessageIDIsZero()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var userAPI      = new Mock<IDiscordRestUserAPI>();
        var channelAPI   = new Mock<IDiscordRestChannelAPI>();

        var reminderService = new ReminderService(mediatorMock.Object, new ShardIdentification(0, 1), Mock.Of<IDiscordRestUserAPI>(), Mock.Of<IDiscordRestChannelAPI>(), NullLogger<ReminderService>.Instance);
        
        userAPI.Setup(u => u.CreateDMAsync(It.IsAny<Snowflake>(), default))
               .ReturnsAsync(new Channel(new(1337), ChannelType.DM));

        mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<IEnumerable<ReminderEntity>>>(), default))
                    .ReturnsAsync(() => new[]
                     {
                         new ReminderEntity
                             { OwnerID = new(69) }
                     });

        // Act
        await reminderService.StartAsync(default);
        await reminderService.StopAsync(default);

        // Assert
        userAPI.Verify(u => u.CreateDMAsync(new(69, default), default), Times.Once);

        channelAPI.Verify(c => c.CreateMessageAsync
                              (
                               It.IsAny<Snowflake>(),
                               It.IsAny<Optional<string>>(),
                               default, default, default,
                               default, default, default,
                               default, default, default, default), Times.Once);
    }

    [Test]
    public async Task CorrectlyRepliesToSourceMessage()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var userAPI      = new Mock<IDiscordRestUserAPI>();
        var channelAPI   = new Mock<IDiscordRestChannelAPI>();

        var reminderService = new ReminderService(mediatorMock.Object, new ShardIdentification(0, 1), Mock.Of<IDiscordRestUserAPI>(), Mock.Of<IDiscordRestChannelAPI>(), NullLogger<ReminderService>.Instance);
        
        userAPI.Setup(u => u.CreateDMAsync(It.IsAny<Snowflake>(), default))
               .ReturnsAsync(new Channel(new(1337), ChannelType.DM));

        channelAPI.Setup(c => c.GetChannelMessageAsync(It.IsAny<Snowflake>(), It.IsAny<Snowflake>(), default))
                  .ReturnsAsync(Result<IMessage>.FromSuccess(Mock.Of<IMessage>())); //There's no actual reason to use Mock.Of<T> other than saving a bunch of default,.

        mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<IEnumerable<ReminderEntity>>>(), default))
                    .ReturnsAsync(() => new[]
                     {
                         new ReminderEntity
                             { OwnerID = new(69), MessageID = new (420) }
                     });

        // Act
        await reminderService.StartAsync(default);
        await reminderService.StopAsync(default);

        // Assert
        channelAPI.Verify(c => c.GetChannelMessageAsync(It.IsAny<Snowflake>(), new(420, default), default), Times.Once);

    }

    [Test]
    public async Task DispatchesExpiredRemindersImmediately()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();

        var userAPI = new Mock<IDiscordRestUserAPI>();

        userAPI.Setup(m => m.CreateDMAsync(It.IsAny<Snowflake>(), default))
               .ReturnsAsync(Result<IChannel>.FromSuccess(Mock.Of<IChannel>()));

        mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<IEnumerable<ReminderEntity>>>(), default))
                    .ReturnsAsync(new[] { new ReminderEntity { ExpiresAt = DateTime.MinValue } });

        var reminderService = new ReminderService(mediatorMock.Object, new ShardIdentification(0, 1), Mock.Of<IDiscordRestUserAPI>(), Mock.Of<IDiscordRestChannelAPI>(), NullLogger<ReminderService>.Instance);

        // Act
        await reminderService.StartAsync(default);
        await reminderService.StopAsync(default);

        // Assert
        mediatorMock.Verify(x => x.Send(It.IsAny<GetAllReminders.Request>(), default), Times.Once);

        mediatorMock.Verify(x => x.Send(It.IsAny<RemoveReminder.Request>(), default), Times.Once);
    }

    [Test]
    public async Task LogsErrorWhenRecipientUnavailable()
    {
        var loggerMock = new Mock<ILogger<ReminderService>>();

        var mediatorMock = new Mock<IMediator>();

        var userAPI = new Mock<IDiscordRestUserAPI>();

        userAPI.Setup(m => m.CreateDMAsync(It.IsAny<Snowflake>(), default))
               .ReturnsAsync(Result<IChannel>.FromError(new NotFoundError()));

        mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<IEnumerable<ReminderEntity>>>(), default))
                    .ReturnsAsync(new[] { new ReminderEntity { ExpiresAt = DateTime.MinValue } });

        var reminderService = new ReminderService(mediatorMock.Object, new ShardIdentification(0, 1), Mock.Of<IDiscordRestUserAPI>(), Mock.Of<IDiscordRestChannelAPI>(), NullLogger<ReminderService>.Instance);
        
        // Act
        await reminderService.StartAsync(default);
        await reminderService.StopAsync(default);

        // Assert
        loggerMock.Verify(
                          x => x.Log
                              (
                               LogLevel.Error,
                               It.IsAny<EventId>(),
                               It.IsAny<It.IsAnyType>(),
                               null,
                               It.IsAny<Func<It.IsAnyType, Exception, string>>()
                              ),
                          Times.Once);

        mediatorMock.Verify(x => x.Send(It.IsAny<RemoveReminder.Request>(), default), Times.Once);
    }

    [Test]
    public async Task LogsErrorWhenRecipientDMClosed()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ReminderService>>();

        var mediatorMock = new Mock<IMediator>();

        var userAPI = new Mock<IDiscordRestUserAPI>();

        var channelAPI = new Mock<IDiscordRestChannelAPI>();

        channelAPI.Setup(m => m.CreateMessageAsync
                             (
                              It.IsAny<Snowflake>(),
                              It.IsAny<Optional<string>>(),
                              default, default, default,
                              default, default, default,
                              default, default, default, default))
                  .ReturnsAsync(Result<IMessage>.FromError(new NotFoundError()));

        userAPI.Setup(m => m.CreateDMAsync(It.IsAny<Snowflake>(), default))
               .ReturnsAsync(Result<IChannel>.FromSuccess(Mock.Of<IChannel>()));

        mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<IEnumerable<ReminderEntity>>>(), default))
                    .ReturnsAsync(new[] { new ReminderEntity { ExpiresAt = DateTime.MinValue } });

        var reminderService = new ReminderService(mediatorMock.Object, new ShardIdentification(0, 1), Mock.Of<IDiscordRestUserAPI>(), Mock.Of<IDiscordRestChannelAPI>(), NullLogger<ReminderService>.Instance);

        // Act
        await reminderService.StartAsync(default);
        await reminderService.StopAsync(default);

        // Assert
        loggerMock.Verify(
                          x => x.Log
                              (
                               LogLevel.Error,
                               It.IsAny<EventId>(),
                               It.IsAny<It.IsAnyType>(),
                               null,
                               It.IsAny<Func<It.IsAnyType, Exception, string>>()
                              ),
                          Times.Once);

    }

}