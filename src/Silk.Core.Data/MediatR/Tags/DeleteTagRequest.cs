﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Silk.Core.Data.Entities;

namespace Silk.Core.Data.MediatR.Tags
{
    /// <summary>
    /// Request to delete a <see cref="TagEntity" />.
    /// </summary>
    public record DeleteTagRequest(string Name, ulong GuildId) : IRequest;

    /// <summary>
    /// The default handler for <see cref="DeleteTagRequest" />.
    /// </summary>
    public class DeleteTagHandler : IRequestHandler<DeleteTagRequest>
    {
        private readonly GuildContext _db;
        public DeleteTagHandler(GuildContext db) => _db = db;

        public async Task<Unit> Handle(DeleteTagRequest request, CancellationToken cancellationToken)
        {
            TagEntity tag = await _db
                .Tags
                .Include(t => t.OriginalTag)
                .Include(t => t.Aliases)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t =>
                    t.Name.ToLower() == request.Name.ToLower() &&
                    t.GuildId == request.GuildId, cancellationToken);

            TagEntity[] aliasedTags = await _db
                .Tags
                .Include(t => t.OriginalTag)
                .Include(t => t.Aliases)
                .AsSplitQuery()
                .Where(t => t.OriginalTagId == tag.Id || t.OriginalTag!.OriginalTagId == tag.Id)
                .ToArrayAsync(cancellationToken);

            _db.Tags.Remove(tag);
            _db.Tags.RemoveRange(aliasedTags);
            
            await _db.SaveChangesAsync(cancellationToken);
            return default;
        }
    }
}