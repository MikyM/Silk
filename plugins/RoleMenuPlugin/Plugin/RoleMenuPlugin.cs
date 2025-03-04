﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Interactivity.Extensions;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Abstractions.Attributes;
using Remora.Results;
using RoleMenuPlugin.Conditions;
using RoleMenuPlugin.Database;
using RoleMenuPlugin.Parsers;

[assembly: RemoraPlugin(typeof(RoleMenuPlugin.RoleMenuPlugin))]

namespace RoleMenuPlugin;

public sealed class RoleMenuPlugin : PluginDescriptor, IMigratablePlugin
{
    public override string Name        => "Role-Menu Plugin";
    public override string Description => "Provides interaction-based role-menu functionality.";

    public override Result ConfigureServices(IServiceCollection serviceCollection)
    {
        try
        {
            serviceCollection
               .AddMediatR(typeof(RoleMenuPlugin))
               .AddResponder<RoleMenuButtonFixer>()
               .AddInteractionGroup<RoleMenuInteractionCommands>()
               .AddCommandTree()
               .WithCommandGroup<RoleMenuCommand>()
               .Finish()
               .AddCondition<RoleMenuCondition>()
               .AddParser<MessageLinkSnowflakeParser>()
               .AddDbContext<RoleMenuContext>((s, b) =>
                {
                    var config = s.GetRequiredService<IConfiguration>();
                    var dbString = config
                                  .GetSection("Plugins")
                                  .GetSection("RoleMenu")
                                  .GetSection("Database")
                                  .Value ?? throw new KeyNotFoundException("Missing plugin config!");

                    b.UseNpgsql(dbString);
                });
        }
        catch (Exception e)
        {
            return Result.FromError(new ExceptionError(e));
        }
        
        return Result.FromSuccess();
    }

    public async Task<Result> MigrateAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        var context = serviceProvider.GetRequiredService<RoleMenuContext>();

        try
        {
            await context.Database.MigrateAsync(ct);
        }
        catch (Exception e)
        {
            return Result.FromError(new ExceptionError(e));
        }
        
        return Result.FromSuccess();
    }

    public override ValueTask<Result> InitializeAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        serviceProvider.GetRequiredService<ILogger<RoleMenuPlugin>>().LogInformation("Silk! RoleMenu plugin {Version} loaded!", Version.ToString(3));
        
        return new(Result.FromSuccess());
    }

}