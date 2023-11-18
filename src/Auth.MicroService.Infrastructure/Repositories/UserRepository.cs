﻿using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Repositories;
using Auth.MicroService.Infrastructure.Context;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _authDbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task AddNewUser(User user, CancellationToken ct)
        {
            await _authDbContext.Set<User>().AddAsync(user, ct);
            await _authDbContext.SaveChangesAsync(ct);
        }
    }
}
