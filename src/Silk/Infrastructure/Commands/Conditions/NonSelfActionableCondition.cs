using System.Threading;
using System.Threading.Tasks;
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Results;
using Silk.Errors;

namespace Silk.Commands.Conditions;

public class NonSelfActionableCondition :
    ICondition<NonSelfActionableAttribute, IUser>,
    ICondition<NonSelfActionableAttribute, IGuildMember>
{
    private readonly ICommandContext     _context;
    private readonly IDiscordRestUserAPI _users;
    
    public NonSelfActionableCondition(ICommandContext context, IDiscordRestUserAPI users)
    {
        _context = context;
        _users   = users;
    }

    public async ValueTask<Result> CheckAsync(NonSelfActionableAttribute attribute, IUser user, CancellationToken ct = default)
    {
        if (user.ID == _context.User.ID)
            return Result.FromError(new SelfActionError("Sorry, but I can't let you do this to yourself!"));

        var selfResult = await _users.GetCurrentUserAsync(ct);
        
        if (!selfResult.IsDefined(out var self))
            return Result.FromError(selfResult.Error!); 
        
        if (self.ID == _context.User.ID)
            return Result.FromError(new SelfActionError("Sorry, but it's against my programming to do this to myself!"));
        
        return Result.FromSuccess();
    }


    public ValueTask<Result> CheckAsync(NonSelfActionableAttribute attribute, IGuildMember member, CancellationToken ct = default)
        => member.User.IsDefined(out var user)
            ? CheckAsync(attribute, user, ct)
            : ValueTask.FromResult(Result.FromError(new InvalidOperationError("The member does not contain a user.")));
}