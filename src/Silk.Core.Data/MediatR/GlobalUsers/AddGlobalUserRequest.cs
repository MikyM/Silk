﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Silk.Core.Data.Entities;

namespace Silk.Core.Data.MediatR.GlobalUsers
{
    /// <summary>
    /// Request for adding a user who's information is tracked globally rather than per-guild.
    /// </summary>
    public record AddGlobalUserRequest(ulong UserId, int? Cash) : IRequest<GlobalUserEntity>;

    /// <summary>
    /// The default handler for <see cref="AddGlobalUserRequest" />.
    /// </summary>
    public class AddGlobalUserHandler : IRequestHandler<AddGlobalUserRequest, GlobalUserEntity>
    {
        private readonly GuildContext _db;
        public AddGlobalUserHandler(GuildContext db)
        {
            _db = db;
        }
        public async Task<GlobalUserEntity> Handle(AddGlobalUserRequest request, CancellationToken cancellationToken)
        {
            GlobalUserEntity user = new()
            {
                Id = request.UserId,
                Cash = request.Cash ?? 0,
                LastCashOut = DateTime.MinValue
            };
            _db.GlobalUsers.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
            return user;
        }
    }
}