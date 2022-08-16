using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Interject;
using MediatR;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;
using Silk.Data.MediatR.Users;
using Silk.Data.MediatR.Users.History;
using Silk.Services.Data;

namespace Silk.Responders;

[ResponderGroup(ResponderGroup.Late)]
public class MemberDataCacherResponder //: IResponder<IGuildMemberAdd>, IResponder<IGuildMemberRemove>
{
    private readonly IInterjector            _mediator;
    private readonly GuildConfigCacheService _config;
    public MemberDataCacherResponder(IInterjector mediator, GuildConfigCacheService config)
    {
        _mediator = mediator;
        _config = config;
    }

    public async Task<Result> RespondAsync(IGuildMemberAdd gatewayEvent, CancellationToken ct = default)
    {
        var config = await _config.GetConfigAsync(gatewayEvent.GuildID);
        
        if (!config.Logging.LogMemberJoins)
            return Result.FromSuccess();
        
        var cacheResult = await _mediator.SendAsync(new GetOrCreateUser.Request(gatewayEvent.GuildID, gatewayEvent.User.Value.ID, JoinedAt: gatewayEvent.JoinedAt), ct);

        if (cacheResult.IsDefined(out var user) && user.History.Last().Date != gatewayEvent.JoinedAt)
            await _mediator.SendAsync(new AddUserJoinDate.Request(gatewayEvent.GuildID, user.ID, gatewayEvent.JoinedAt), ct);

        return (Result)cacheResult;
    }

    public async Task<Result> RespondAsync(IGuildMemberRemove gatewayEvent, CancellationToken ct = default)
    {
        var config = await _config.GetConfigAsync(gatewayEvent.GuildID);
        
        if (!config.Logging.LogMemberLeaves)
            return Result.FromSuccess();
        
        var cacheResult = await _mediator.SendAsync(new GetOrCreateUser.Request(gatewayEvent.GuildID, gatewayEvent.User.ID, JoinedAt: DateTimeOffset.MinValue), ct);
        
        if (cacheResult.IsDefined(out var user))
            await _mediator.SendAsync(new AddUserLeaveDate.Request(gatewayEvent.GuildID, user.ID, DateTimeOffset.UtcNow), ct);
        
        return (Result)cacheResult;
    }
}