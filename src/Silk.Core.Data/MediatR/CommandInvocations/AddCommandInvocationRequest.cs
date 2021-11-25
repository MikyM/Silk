﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Silk.Core.Data.Entities;

namespace Silk.Core.Data.MediatR.CommandInvocations
{
    /// <summary>
    /// Request for adding a <see cref="CommandInvocationEntity" />.
    /// </summary>
    public record AddCommandInvocationRequest(ulong UserId, ulong? GuildId, string CommandName) : IRequest;

    /// <summary>
    /// The default handler for <see cref="AddCommandInvocationRequest" />.
    /// </summary>
    public class AddCommandInvocationHandler : IRequestHandler<AddCommandInvocationRequest>
    {
        private readonly GuildContext _db;

        public AddCommandInvocationHandler(GuildContext db)
        {
            _db = db;
        }

        public async Task<Unit> Handle(AddCommandInvocationRequest request, CancellationToken cancellationToken)
        {
            CommandInvocationEntity command = new()
            {
                UserId = request.UserId, 
                GuildId = request.GuildId, 
                CommandName = request.CommandName
            };

            _db.CommandInvocations.Add(command);

            await _db.SaveChangesAsync(cancellationToken);
            return new();
        }
    }
}