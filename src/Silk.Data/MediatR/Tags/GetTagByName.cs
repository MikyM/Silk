﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Remora.Rest.Core;
using Silk.Data.Entities;

namespace Silk.Data.MediatR.Tags;

public static class GetTagByName
{
    /// <summary>
    /// Request for getting tags by name, or null, if none are found.
    /// </summary>
    public sealed record Request(string Name, Snowflake GuildID) : IRequest<IEnumerable<TagEntity>>;

    /// <summary>
    /// The default handler for <see cref="Request" />.
    /// </summary>
    internal sealed class Handler : IRequestHandler<Request, IEnumerable<TagEntity>>
    {
        private readonly GuildContext _db;

        public Handler(GuildContext db) => _db = db;

        public async Task<IEnumerable<TagEntity>> Handle(Request request, CancellationToken cancellationToken)
        {
            TagEntity[] tags = await _db
                                    .Tags
                                    .Include(t => t.Aliases)
                                    .Include(t => t.OriginalTag)
                                    .AsSplitQuery()
                                    .Where(t => EF.Functions.Like(t.Name.ToLower(), request.Name.ToLower() + '%'))
                                    .Where(t => t.GuildID == request.GuildID)
                                    .ToArrayAsync(cancellationToken);

            return tags;
        }
    }
}