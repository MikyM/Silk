﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NUnit.Framework;
using Remora.Rest.Core;
using Respawn;
using Respawn.Graph;
using Silk.Data.Entities;
using Silk.Data.MediatR.Guilds;
using Silk.Data.MediatR.Users;

namespace Silk.Data.Tests.MediatR;

public class BulkUserTests
{
    private readonly Snowflake          GuildId          = new (10);
    private const    string             ConnectionString = "Server=localhost; Port=5432; Database=unit_test; Username=silk; Password=silk; Include Error Detail=true;";
    private readonly Checkpoint         _checkpoint      = new() { TablesToIgnore = new Table[] { "guilds", "__EFMigrationsHistory" }, DbAdapter = DbAdapter.Postgres };
    private readonly IServiceCollection _provider        = new ServiceCollection();

    private GuildContext _context;

    private IMediator _mediator;

    [OneTimeSetUp]
    public async Task GlobalSetUp()
    {
        _provider.AddDbContext<GuildContext>(o => o.UseNpgsql(ConnectionString), ServiceLifetime.Transient);
        _provider.AddMediatR(typeof(GuildContext));
        _mediator = _provider.BuildServiceProvider().GetRequiredService<IMediator>();

        _context = _provider.BuildServiceProvider().GetRequiredService<GuildContext>();
        await _context.Database.MigrateAsync();
        _context.Guilds.Add(new() { ID = GuildId });
        await _context.SaveChangesAsync();
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        if (_context.Guilds.Any())
        {
            _context.ChangeTracker.Clear();
            _context.Guilds.RemoveRange(_context.Guilds);
            await _context.SaveChangesAsync();
        }
        await _context.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _checkpoint.Reset(connection);
    }

    [Test]
    public async Task InsertsAllUsers()
    {
        //Arrange
        List<(Snowflake, DateTimeOffset)> users = new()
        {
            (new(1), default),
            (new(2), default)
        };

        //Act
        await _mediator.Send(new BulkAddUserToGuild.Request(users,GuildId));
        
        var result = _context.Users.Count();
        //Assert
        Assert.AreEqual(users.Count, result);
    }

    [Test]
    public async Task InsertsAndUpdatesAllUsers()
    {
        //Arrange
        await _mediator.Send(new GetOrCreateUser.Request(GuildId, new(1)));
        List<(Snowflake, DateTimeOffset)> users = new()
        {
            (new(1), default),
            (new(2), default)
        };

        //Act
        await _mediator.Send(new BulkAddUserToGuild.Request(users, GuildId));
        var result = _context.Users.ToArray().Length;

        //Assert
        Assert.AreEqual(users.Count, result);
    }

    [Test]
    public async Task UpdatesUserForMultipleGuilds()
    {
        await _mediator.Send(new GetOrCreateUser.Request(GuildId, new(1)));
        await _mediator.Send(new GetOrCreateGuild.Request(new(20), "??"));

        await _mediator.Send(new BulkAddUserToGuild.Request(new[] { (new Snowflake(1), default(DateTimeOffset)) }, new(20)));

        var snowflake = new Snowflake(1);
        
        var result = _context.Users.Include(u => u.Guilds).First(u => u.ID == snowflake);
        
        Assert.AreEqual(2, result.Guilds.Count);
    }
}