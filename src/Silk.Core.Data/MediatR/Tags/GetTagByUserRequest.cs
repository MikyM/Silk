﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Silk.Core.Data.Entities;

namespace Silk.Core.Data.MediatR.Tags
{
    /// <summary>
    /// Request to get all <see cref="TagEntity" />'s created by a User in a Guild
    /// </summary>
    /// <param name="GuildId">The Id of the Guild</param>
    /// <param name="OwnerId">The Id of the User</param>
    public record GetTagByUserRequest(ulong GuildId, ulong OwnerId) : IRequest<IEnumerable<TagEntity>>;

    /// <summary>
    /// The default handler for <see cref="GetTagByUserRequest" />.
    /// </summary>
    public class GetTagByUserHandler : IRequestHandler<GetTagByUserRequest, IEnumerable<TagEntity>>
    {
        private readonly GuildContext _db;
        public GetTagByUserHandler(GuildContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TagEntity>> Handle(GetTagByUserRequest request, CancellationToken cancellationToken)
        {
            TagEntity[] tags = await _db
                .Tags
                .Include(t => t.OriginalTag)
                .Include(t => t.Aliases)
                .Where(t => t.GuildId == request.GuildId && t.OwnerId == request.OwnerId)
                .ToArrayAsync(cancellationToken);

            return tags;
        }
    }
}