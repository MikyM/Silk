using System.Linq;
using System.Threading.Tasks;
using Interject;
using Interject.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NUnit.Framework;
using Remora.Rest.Core;
using Respawn;
using Respawn.Graph;
using Silk.Data.Entities;
using Silk.Data.MediatR.Guilds;

namespace Silk.Data.Tests.MediatR;

public class GuildTests
{
    private readonly Snowflake          GuildId          = new(10);
    private const    string             ConnectionString = "Server=localhost; Port=5432; Database=unit_test; Username=silk; Password=silk; Include Error Detail=true;";
    private readonly Checkpoint         _checkpoint      = new() { TablesToIgnore = new Table[] { "__EFMigrationsHistory" }, DbAdapter = DbAdapter.Postgres };
    private readonly ServiceCollection _provider        = new ServiceCollection();

    private GuildContext _context;

    private IInterjector _mediator;

    [OneTimeSetUp]
    public async Task GlobalSetUp()
    {
        _provider.AddDbContext<GuildContext>(o => o.UseNpgsql(ConnectionString), ServiceLifetime.Transient);
        _provider.AddInterject(ServiceLifetime.Scoped, typeof(GuildContext).Assembly);
        _mediator = _provider.BuildServiceProvider().GetRequiredService<IInterjector>();

        _context = _provider.BuildServiceProvider().GetRequiredService<GuildContext>();
        await _context.Database.MigrateAsync();
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
    public async Task GetOrCreateCreatesGuildCorrectly()
    {
        //Arrange
        GuildEntity result;
        int before,
            after;
        //Act
        before = _context.Guilds.Count();
        result = await _mediator.SendAsync(new GetOrCreateGuild.Request(GuildId, ""));
        after  = _context.Guilds.Count();
        //Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(before, after);
    }

    [Test]
    public async Task GetOrCreateDoesNotRecreate()
    {
        //Arrange
        GuildEntity? result;
        int before,
            after;

        await _mediator.SendAsync(new GetOrCreateGuild.Request(GuildId, ""));

        //Act
        before = _context.Guilds.Count();
        result = await _mediator.SendAsync(new GetOrCreateGuild.Request(GuildId, ""));
        after  = _context.Guilds.Count();

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(before, after);
    }

    [Test]
    public async Task GetReturnsNullForNonGuild()
    {
        //Arrange
        GuildEntity? result;
        //Act
        result = await _mediator.SendAsync(new GetGuild.Request(GuildId));
        //Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetReturnsGuildCorrectly()
    {
        //Arrange
        GuildEntity? result;
        await _mediator.SendAsync(new GetOrCreateGuild.Request(GuildId, ""));

        //Act
        result = await _mediator.SendAsync(new GetGuild.Request(GuildId));

        //Assert
        Assert.IsNotNull(result);

    }
}