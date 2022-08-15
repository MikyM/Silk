using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Silk.Data.Entities;

namespace Silk.Data.MediatR.Greetings;

public static class GetPendingGreetings
{
    public record Request(int ShardCount, int ShardID) : IRequest<IReadOnlyList<PendingGreetingEntity>>;
    
    internal class Handler : IRequestHandler<Request, IReadOnlyList<PendingGreetingEntity>>
    {
        private readonly IDbContextFactory<GuildContext> _dbFactory;
        
        public Handler(IDbContextFactory<GuildContext> dbFactory) => _dbFactory = dbFactory;


        public async Task<IReadOnlyList<PendingGreetingEntity>> Handle(Request request, CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
            
            return await db.PendingGreetings
                           .FromSqlRaw("SELECT * FROM pending_greetings pg WHERE (pg.guild_id::bigint >> 22) % {0} = {1}", request.ShardCount, request.ShardID)
                           .ToArrayAsync(cancellationToken); // Will EF Core do client eval for this?
        }
    }

}