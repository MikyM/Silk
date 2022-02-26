using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Remora.Results;

namespace Silk.Data.MediatR.Greetings;

public static class RemovePendingGreeting
{
    public record Request(int ID) : IRequest<Result>;
    
    internal class Handler : IRequestHandler<Request, Result>
    {
        private readonly GuildContext _db;

        public Handler(GuildContext db) => _db = db;

        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var greeting = await _db.PendingGreetings.FirstOrDefaultAsync(x => x.Id == request.ID, cancellationToken);

            if (greeting is null)
                return Result.FromError(new NotFoundError());
            
            _db.PendingGreetings.Remove(greeting);
            
            await _db.SaveChangesAsync(cancellationToken);

            return Result.FromSuccess();
        }
    }
}