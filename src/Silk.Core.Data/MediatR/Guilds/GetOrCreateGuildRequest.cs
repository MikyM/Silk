﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Silk.Core.Data.Entities;

namespace Silk.Core.Data.MediatR.Guilds
{
    /// <summary>
    /// Request for retrieving or creating a <see cref="GuildEntity" />.
    /// </summary>
    /// <param name="GuildId">The Id of the Guild</param>
    /// <param name="Prefix">The prefix of the Guild</param>
    public record GetOrCreateGuildRequest(ulong GuildId, string Prefix) : IRequest<GuildEntity>;

    /// <summary>
    /// The default handler for <see cref="GetOrCreateGuildRequest" />.
    /// </summary>
    public class GetOrCreateGuildHandler : IRequestHandler<GetOrCreateGuildRequest, GuildEntity>
    {
        private readonly GuildContext _db;
        private readonly IMediator _mediator;

        public GetOrCreateGuildHandler(GuildContext db, IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
        }

        public async Task<GuildEntity> Handle(GetOrCreateGuildRequest request, CancellationToken cancellationToken)
        {
            GuildEntity? guild = await _db.Guilds
                .Include(g => g.Users)
                .Include(g => g.Infractions)
                .AsSplitQuery()
                .FirstOrDefaultAsync(g => g.Id == request.GuildId, cancellationToken);

            if (guild is not null)
                return guild;

            guild = await _mediator.Send(new AddGuildRequest(request.GuildId, request.Prefix), cancellationToken);
            
            return guild;
        }
    }
}