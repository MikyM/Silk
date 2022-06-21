using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Remora.Commands.Results;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Results;
using Remora.Discord.Commands.Services;
using Remora.Results;
using Sentry;
using Silk.Errors;
using Silk.Extensions.Remora;
using Silk.Services.Bot.Help;

namespace Silk;

public class PostCommandHandler : IPostExecutionEvent
{
    private readonly IHub                       _hub;
    private readonly ICommandHelpService        _help;
    private readonly ICommandPrefixMatcher      _prefix;
    private readonly IDiscordRestChannelAPI     _channels;
    
    private readonly IDiscordRestInteractionAPI _interactions;

    public PostCommandHandler
    (
        IHub                       hub,
        ICommandHelpService        help,
        ICommandPrefixMatcher      prefix,
        IDiscordRestChannelAPI     channels,
        IDiscordRestInteractionAPI interactions
    )
    {
        _hub      = hub;
        _help     = help;
        _prefix   = prefix;
        _channels = channels;
        _interactions      = interactions;
    }

    public async Task<Result> AfterExecutionAsync(ICommandContext context, IResult commandResult, CancellationToken ct = default)
    {
        if (commandResult.IsSuccess)
            return Result.FromSuccess();
        
        var error = commandResult.GetDeepestError();
        
        _hub.ConfigureScope(s => s.Contexts[context.User.ID.ToString()] = new Dictionary<string, object>
        { 
            ["id"]       = context.User.ID.ToString(),
            ["guild_id"] = context.GuildID.IsDefined(out var gid) ? gid.ToString() : "DM",
        });

        if (context is MessageContext mc)
        {
            var prefixResult = await _prefix.MatchesPrefixAsync(mc.Message.Content.Value, ct);
        
            if (!prefixResult.IsDefined(out var prefix) || !prefix.Matches || mc.Message.Content.Value.Length <= prefix.ContentStartIndex)
                return Result.FromSuccess();
            
            if (error is CommandNotFoundError)
                await _help.ShowHelpAsync(mc.ChannelID, mc.Message.Content.Value[prefix.ContentStartIndex..]);
        }
        
        if (error is ExceptionError er)
            _hub.CaptureException(er.Exception);

        if (commandResult.Error is AggregateError ag && ag.Errors.First().Error is ConditionNotSatisfiedError)
        {
            var message = error!.Message;

            var responseMessage = error switch
            {
                SelfActionError sae       => sae.Message,
                PermissionDeniedError pne => $"As much as I'd love to, you're missing permissions to {pne.Permissions.Select(p => p.Humanize(LetterCasing.Title)).Humanize()}!",
                _                         => message
            };

            if (context is not InteractionContext ic)
                await _channels.CreateMessageAsync(context.ChannelID, responseMessage, ct: ct);
            else
                await _interactions.CreateFollowupMessageAsync(ic.ApplicationID, ic.Token, responseMessage, flags: MessageFlags.Ephemeral, ct: ct);
        }
        return Result.FromSuccess();
    }
}