﻿using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NUnit.Framework;
using Remora.Rest.Core;
using Respawn;
using Silk.Data.Entities;
using Silk.Data.MediatR.Guilds;

namespace Silk.Data.Tests.MediatR;

public class GuildTests
{
    private readonly Snowflake          GuildId          = new(10);
    private const    string             ConnectionString = "Server=localhost; Port=5432; Database=unit_test; Username=silk; Password=silk; Include Error Detail=true;";
    private readonly Checkpoint         _checkpoint      = new() { TablesToIgnore = new[] { "__EFMigrationsHistory" }, DbAdapter = DbAdapter.Postgres };
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
        _context.Database.Migrate();
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        _context.Guilds.RemoveRange(_context.Guilds);
        await _context.SaveChangesAsync();
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
    public async Task MediatR_Get_Or_Create_Creates_When_Guild_Does_Not_Exist()
    {
        //Arrange
        GuildEntity result;
        int before,
            after;
        //Act
        before = _context.Guilds.Count();
        result = await _mediator.Send(new GetOrCreateGuild.Request(GuildId, ""));
        after  = _context.Guilds.Count();
        //Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(before, after);
    }

    [Test]
    public async Task MediatR_Get_Or_Create_Does_Not_Create_When_Guild_Exists()
    {
        //Arrange
        GuildEntity? result;
        int before,
            after;

        await _mediator.Send(new GetOrCreateGuild.Request(GuildId, ""));

        //Act
        before = _context.Guilds.Count();
        result = await _mediator.Send(new GetOrCreateGuild.Request(GuildId, ""));
        after  = _context.Guilds.Count();

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(before, after);
    }

    [Test]
    public async Task MediatR_Get_Returns_Null_When_Guild_Does_Not_Exist()
    {
        //Arrange
        GuildEntity? result;
        //Act
        result = await _mediator.Send(new GetGuild.Request(GuildId));
        //Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task MediatR_Get_Returns_NonNull_When_Guild_Exists()
    {
        //Arrange
        GuildEntity? result;
        await _mediator.Send(new GetOrCreateGuild.Request(GuildId, ""));

        //Act
        result = await _mediator.Send(new GetGuild.Request(GuildId));

        //Assert
        Assert.IsNotNull(result);

    }
}