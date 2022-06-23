using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Caching;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Caching.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Rest;
using Remora.Results;
using Silk.Extensions.Remora;
using Silk.Services.Data;

namespace Silk.Responders;

[ResponderGroup(ResponderGroup.Late)]
public class GuildMemberRequesterResponder : IResponder<IGuildCreate>
{
    private static readonly SemaphoreSlim _sync = new(1);
    
    private readonly ICacheProvider                         _provider;
    private readonly IRestHttpClient                        _client;
    private readonly GuildCacherService                     _memberCacher;
    private readonly ILogger<GuildMemberRequesterResponder> _logger;

    public GuildMemberRequesterResponder
    (
        ICacheProvider                         provider,
        IRestHttpClient                        client,
        GuildCacherService                     memberCacher,
        ILogger<GuildMemberRequesterResponder> logger
    )
    {
        _provider     = provider;
        _client       = client;
        _memberCacher = memberCacher;
        _logger       = logger;
    }
    
    public async Task<Result> RespondAsync(IGuildCreate gatewayEvent, CancellationToken ct = default)
    {
        try
        {
            await _sync.WaitAsync(ct);
            
            if (gatewayEvent.IsUnavailable.IsDefined(out var unavailable) && unavailable)
                return Result.FromSuccess(); // Thanks, Night.

            var memberResult = await _client.GetGuildMembersAsync(_provider, gatewayEvent.ID);

            if (memberResult.IsDefined(out var members))
            {
                await _memberCacher.CacheMembersAsync(gatewayEvent.ID, members);
            }
            else
            {
                _logger.LogError("Failed to grab members for {GuildID}", gatewayEvent.ID);
            }

            return Result.FromSuccess();
        }
        finally
        {
            _sync.Release();
        }
    }
}