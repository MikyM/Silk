using System.Threading;
using System.Threading.Tasks;
using Interject;
using Interject.Contracts;
using Remora.Rest.Core;
using Silk.Data.Entities;

namespace Silk.Data.MediatR.Guilds;

public static class GetOrCreateGuildConfig
{
    /// <summary>
    /// Request for retrieving or creating a <see cref="GuildConfigEntity" />.
    /// </summary>
    /// <param name="GuildID">The Id of the Guild</param>
    /// <param name="Prefix">The prefix of the Guild</param>
    public sealed record Request(Snowflake GuildID, string Prefix) : IRequest<GuildConfigEntity>;

    /// <summary>
    /// The default handler for <see cref="Request" />.
    /// </summary>
    internal sealed class Handler : IRequestHandler<Request, GuildConfigEntity>
    {
        private readonly IInterjector _mediator;

        public Handler(IInterjector mediator) => _mediator = mediator;

        public async Task<GuildConfigEntity> Handle(Request request, CancellationToken cancellationToken)
        {
            GuildConfigEntity? guildConfig = await _mediator.SendAsync(new GetGuildConfig.Request(request.GuildID), cancellationToken);

            if (guildConfig is not null)
                return guildConfig;
        
            var response = await _mediator.SendAsync(new GetOrCreateGuild.Request(request.GuildID, request.Prefix), cancellationToken);

            guildConfig = response.Configuration;

            return guildConfig;
        }
    }
}