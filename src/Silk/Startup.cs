﻿/*
 * NOTE: THIS IS HERE FOR REFERNCE ONLY.
 */


/*using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PluginLoader.Unity;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Templates;
using Silk.AutoMod;
using Silk.Data;
using Silk.EventHandlers;
using Silk.EventHandlers.Guilds;
using Silk.EventHandlers.MemberAdded;
using Silk.EventHandlers.MemberRemoved;
using Silk.EventHandlers.Messages;
using Silk.EventHandlers.Messages.AutoMod;
using Silk.Services;
using Silk.Services.Bot;
using Silk.Services.Data;
using Silk.Services.Interfaces;
using Silk.Services.Server;
using Silk.SlashCommands;
using Silk.Utilities;
using Silk.Utilities.Bot;
using Silk.Utilities.HttpClient;
using Silk.Extensions;
using Silk.Shared;
using Silk.Shared.Configuration;
using Silk.Shared.Constants;
using Unity;
using Unity.Microsoft.DependencyInjection;
using Unity.Microsoft.Logging;
using YumeChan.PluginBase.Tools.Data;

namespace Silk
{
public static class Startup
{


	private static readonly List<Type> _startupTypes = new();
	public static IUnityContainer Container { get; private set; }

	public static async Task Main()
	{
		// Make Generic Host here. //
		IHostBuilder builder = CreateBuilder();

		ConfigureServices(builder);

		IHost builtBuilder = builder.UseConsoleLifetime().Build();
		DiscordConfigurations.CommandsNext.Services = builtBuilder.Services; // Prevents double initialization of services. //
		DiscordConfigurations.SlashCommands.Services = builtBuilder.Services;

		ConfigureDiscordClient(builtBuilder.Services);
		await EnsureDatabaseCreatedAndApplyMigrations(builtBuilder);

		Container = builtBuilder.Services.Get<IUnityContainer>()!;

		foreach (var service in _startupTypes)
			_ = Container.Resolve(service);

		await builtBuilder.RunAsync().ConfigureAwait(false);
	}

	private static async Task EnsureDatabaseCreatedAndApplyMigrations(IHost builtBuilder)
	{
		try
		{
			using IServiceScope? serviceScope = builtBuilder.Services?.CreateScope();
			if (serviceScope is not null)
			{
				await using GuildContext? dbContext = serviceScope.ServiceProvider
					.GetRequiredService<IDbContextFactory<GuildContext>>()
					.CreateDbContext();

				IEnumerable<string>? pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

				if (pendingMigrations.Any())
					await dbContext.Database.MigrateAsync();
			}
		}
		catch (Exception)
		{
			/* Ignored. Todo: Probably should handle? #1#
		}
	}

	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "EFCore CLI tools rely on reflection.")]
	public static IHostBuilder CreateHostBuilder(string[] args)
	{
		var builder = CreateBuilder();
		builder.ConfigureServices((context, container) =>
		{
			SilkConfigurationOptions? silkConfig = context.Configuration.GetSilkConfigurationOptionsFromSection();
			AddDatabases(container, silkConfig.Persistence);
		});

		return builder;
	}

	private static IHostBuilder CreateBuilder()
	{
		IHostBuilder? builder = Host.CreateDefaultBuilder();

		builder.ConfigureAppConfiguration((_, configuration) =>
		{
			configuration.SetBasePath(Directory.GetCurrentDirectory());
			configuration.AddJsonFile("appSettings.json", true, false);
			configuration.AddUserSecrets<Main>(true, false);
		});
		return builder;
	}

	private static void AddLogging(HostBuilderContext host)
	{
		LoggerConfiguration? logger = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.WriteTo.Console(new ExpressionTemplate(StringConstants.LogFormat, theme: SilkLogTheme.TemplateTheme))
			.WriteTo.File("./logs/silkLog.log", LogEventLevel.Verbose, StringConstants.FileLogFormat, retainedFileCountLimit: null, rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.FromMinutes(1))
			.MinimumLevel.Override("Microsoft", LogEventLevel.Error)
			.MinimumLevel.Override("DSharpPlus", LogEventLevel.Warning)
			.MinimumLevel.Override("System.Net", LogEventLevel.Fatal);

		SilkConfigurationOptions? configOptions = host.Configuration.GetSilkConfigurationOptionsFromSection();
		Log.Logger = configOptions.LogLevel switch
		{
			"All" => logger.MinimumLevel.Verbose().CreateLogger(),
			"Info" => logger.MinimumLevel.Information().CreateLogger(),
			"Debug" => logger.MinimumLevel.Debug().CreateLogger(),
			"Warning" => logger.MinimumLevel.Warning().CreateLogger(),
			"Error" => logger.MinimumLevel.Error().CreateLogger(),
			"Panic" => logger.MinimumLevel.Fatal().CreateLogger(),
			_ => logger.MinimumLevel.Verbose().CreateLogger()
		};
		Log.Logger.ForContext(typeof(Startup)).Information("Logging Initialized!");
	}

	private static IHostBuilder ConfigureServices(IHostBuilder builder, bool addServices = true)
	{
		return builder
			.UseUnityServiceProvider()
			.ConfigureLogging(l => l.ClearProviders())
			.UseSerilog()
			.ConfigureContainer<IUnityContainer>((context, container) =>
			{
				var services = new ServiceCollection();
				SilkConfigurationOptions? silkConfig = context.Configuration.GetSilkConfigurationOptionsFromSection();

				AddSilkConfigurationOptions(services, context.Configuration);
				AddDatabases(services, silkConfig.Persistence);

				if (!addServices) return;

				if (silkConfig.Emojis?.EmojiIds is not null)
					silkConfig.Emojis.PopulateEmojiConstants();

				services.AddTransient(typeof(ILogger<>), typeof(Shared.Types.Logger<>));

				services.AddSingleton<DiscordClient>(s => new(new(DiscordConfigurations.Discord) { LoggerFactory = s.GetService<ILoggerFactory>() }));

				services.AddMemoryCache(option => option.ExpirationScanFrequency = TimeSpan.FromSeconds(30));


				services.AddHttpClient(StringConstants.HttpClientName,
					client => client.DefaultRequestHeaders.UserAgent.ParseAdd(
						$"Silk Project by VelvetThePanda / v{StringConstants.Version}"));

				services.Replace(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, CustomLoggingFilter>());

				#region Services

				services.AddSingleton<ConfigService>();
				services.AddSingleton<FlagOverlayService>();

				#endregion

				#region AutoMod

				services.AddSingleton<AntiInviteHelper>();
				services.AddSingleton<AutoModAntiPhisher>();

				#endregion

				services.AddSingleton<BotExceptionHandler>();
				services.AddSingleton<SlashCommandExceptionHandler>();
				services.AddSingleton<SerilogLoggerFactory>();

				#region Unmonitored Services

				services.AddUnmonitoredService<MessageAddAntiInvite>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<RoleAddedHandler>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<RoleRemovedHandler>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<GuildEventHandler>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<MemberGreetingService>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<MessageUpdateHandler>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<CommandHandler>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<MessageAddAntiInvite>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<MessagePhishingDetector>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<AutoModMuteApplier>(ServiceLifetime.Singleton);
				services.AddUnmonitoredService<MemberRemovedHandler>(ServiceLifetime.Singleton);

				#endregion

				services.AddScoped<IPrefixCacheService, PrefixCacheService>();
				services.AddSingleton<IInfractionService, InfractionService>();

				services.AddSingleton<ICacheUpdaterService, CacheUpdaterService>();

				services.AddHostedService(b => b.GetRequiredService<AutoModAntiPhisher>());

				services.AddSingleton<TagService>();

				services.AddSingleton<Main>();
				services.AddHostedService(s => s.GetRequiredService<Main>());

				services.AddSingleton<IInfractionService, InfractionService>();
				services.AddHostedService(s => s.Get<IInfractionService>() as InfractionService);

				// Couldn't figure out how to get the service since AddHostedService adds it as //
				// IHostedService. Google failed me, but https://stackoverflow.com/a/65552373 helped a lot. //
				services.AddSingleton<ReminderService>();
				services.AddHostedService(b => b.GetRequiredService<ReminderService>());

				services.AddHostedService<StatusService>();

				services.AddMediatR(typeof(Main));
				services.AddMediatR(typeof(GuildContext));

				services.AddSingleton<GuildCacher>();

				//services.AddSingleton<UptimeService>();
				//services.AddHostedService(b => b.GetRequiredService<UptimeService>());
				services.RegisterShardedPluginServices();

				services.AddSingleton(typeof(IDatabaseProvider<>), typeof(Types.DatabaseProvider<>));
				container.AddExtension(new LoggingExtension(new SerilogLoggerFactory()));
				container.AddServices(new ServiceCollection()
					.AddLogging(l =>
					{
						l.AddSerilog();
						AddLogging(context);
					}));

				container.AddExtension(new Diagnostic());

				container.AddServices(services);
			});
	}

	private static IServiceCollection AddUnmonitoredService<T>(this IServiceCollection services, ServiceLifetime lifetime)
		=> services.AddUnmonitoredService<T, T>(lifetime);

	private static IServiceCollection AddUnmonitoredService<TRegister, TImplemenation>(this IServiceCollection services, ServiceLifetime lifetime)
	{
		services.Add(new(typeof(TRegister), typeof(TImplemenation), lifetime));

		_startupTypes.Add(typeof(TRegister));

		return services;
	}

	private static void ConfigureDiscordClient(IServiceProvider services)
	{
		var client = DiscordConfigurations.Discord;
		var config = services.Get<IOptions<SilkConfigurationOptions>>()!.Value;

		client.ShardCount = config!.Discord.Shards;
		client.Token = config.Discord.BotToken;
	}

	private static void AddSilkConfigurationOptions(IServiceCollection services, IConfiguration configuration)
	{
		// Add and Bind IOptions configuration for appSettings.json and UserSecrets configuration structure
		// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0
		IConfigurationSection? silkConfigurationSection = configuration.GetSection(SilkConfigurationOptions.SectionKey);
		services.Configure<SilkConfigurationOptions>(silkConfigurationSection);
	}

	private static void AddDatabases(IServiceCollection services, SilkPersistenceOptions persistenceOptions)
	{
		void Builder(DbContextOptionsBuilder b)
		{
			b.UseNpgsql(persistenceOptions.GetConnectionString());
			#if DEBUG
			b.EnableSensitiveDataLogging();
			b.EnableDetailedErrors();
			#endif // EFCore will complain about enabling sensitive data if you're not in a debug build. //
		}

		services.AddDbContext<GuildContext>(Builder, ServiceLifetime.Transient);
		services.AddDbContextFactory<GuildContext>(Builder, ServiceLifetime.Transient);
		//services.TryAdd(new ServiceDescriptor(typeof(GuildContext), p => p.GetRequiredService<IDbContextFactory<GuildContext>>().CreateDbContext(), ServiceLifetime.Transient));
	}
}

/* Todo: Move this class maybe? 
public static class IConfigurationExtensions
{
	/// <summary>
	/// An extension method to get a <see cref="SilkConfigurationOptions" /> instance from the Configuration by Section Key
	/// </summary>
	/// <param name="config">the configuration</param>
	/// <returns>an instance of the SilkConfigurationOptions class, or null if not found</returns>
	public static SilkConfigurationOptions GetSilkConfigurationOptionsFromSection(this IConfiguration config)
	{
		return config.GetSection(SilkConfigurationOptions.SectionKey).Get<SilkConfigurationOptions>();
	}
}
}*/
