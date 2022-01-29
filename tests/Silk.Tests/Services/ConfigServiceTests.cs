﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using Silk.Data.MediatR.Guilds;
using Silk.Services.Data;

namespace Silk.Tests.Services;

public class ConfigServiceTests
{
    private readonly Mock<IMemoryCache>      _cache;
    private readonly GuildConfigCacheService _guildConfigCacheService;
    private readonly Mock<IMediator>         _mediator;

    public ConfigServiceTests()
    {

        _cache = new();
        _cache.Setup(cache => cache.CreateEntry(0ul)).Returns(Mock.Of<ICacheEntry>);
        _mediator = new() { CallBase = false };


        _mediator
           .Setup(m => m.Send(It.IsAny<IRequest<GetGuildConfig.Request>>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(It.IsAny<GetGuildConfig.Request>())
           .Verifiable("uHHHH");

        _guildConfigCacheService = new(_cache.Object, _mediator.Object);
    }

    [Test]
    public async Task GetConfigAsync_WhenInvalidId_RetrievesFromDatabase()
    {
        //Act
        object discard;
        _cache.Setup(cache => cache.TryGetValue(0ul, out discard)).Returns(false);
        await _guildConfigCacheService.GetConfigAsync(new(0));
        //Assert
        _mediator.Verify(x => x.Send(It.IsAny<GetGuildConfig.Request>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetConfigAsync_WhenValidId_RetrievesFromCache()
    {
        object discard;
        _cache.Setup(cache => cache.TryGetValue(0ul, out discard)).Returns(true);
        _mediator.Verify(m => m.Send(new(), It.IsAny<CancellationToken>()), Times.Never);
    }
}