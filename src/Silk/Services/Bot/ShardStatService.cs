using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Gateway;
using Silk.Utilities;
using StackExchange.Redis;

namespace Silk.Services.Bot;

public class ShardStatService : BackgroundService
{
    private readonly Process                _process;
    private readonly DiscordGatewayClient   _client;
    private readonly IShardIdentification   _shard;
    private readonly IConnectionMultiplexer _redis;
    

    public ShardStatService(IOptions<DiscordGatewayClientOptions> options, DiscordGatewayClient client, IConnectionMultiplexer redis)
    {
        _process = Process.GetCurrentProcess();
        _client  = client;
        _shard   = options.Value.ShardIdentification!;
        _redis   = redis;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var db = _redis.GetDatabase();

        var cpuKey = ShardHelper.GetShardCPUUsageStatKey(_shard.ShardID);
        var memKey = ShardHelper.GetShardMemoryStatKey(_shard.ShardID);
        var latKey = ShardHelper.GetShardLatencyStatKey(_shard.ShardID);
        var uptKey = ShardHelper.GetShardUptimeStatKey(_shard.ShardID);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cpu = await GetCPUUsage(stoppingToken);
                var mem = _process.WorkingSet64 / 1024 / 1024;
                var lat = _client.Latency.ToString("hh\\:mm\\:ss");
                var upt = (DateTime.UtcNow - _process.StartTime).ToString("hh\\:mm\\:ss");
                
                await db.StringSetAsync(cpuKey, cpu);
                await db.StringSetAsync(memKey, mem);
                await db.StringSetAsync(latKey, lat);
                await db.StringSetAsync(uptKey, upt);
            }
            catch (OperationCanceledException)
            {
                /* ignored */
            }
        }
    }

    private async Task<double> GetCPUUsage(CancellationToken ct)
    {
        var cpuOld = _process.TotalProcessorTime;

        await Task.Delay(3000, ct);
        
        _process.Refresh();
        
        var cpuNew = _process.TotalProcessorTime;
        
        var cpuDelta = (cpuNew - cpuOld).TotalMilliseconds;

        var usage = cpuDelta / (3000 * Environment.ProcessorCount);
        
        return Math.Truncate(usage * 100);
    }
    
}