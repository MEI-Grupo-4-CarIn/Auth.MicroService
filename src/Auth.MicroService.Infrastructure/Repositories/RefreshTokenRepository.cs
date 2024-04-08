using System;
using System.Threading;
using System.Threading.Tasks;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Repositories;
using Auth.MicroService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Auth.MicroService.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _authDbContext;

        public RefreshTokenRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task AddNewRefreshToken(RefreshToken token, CancellationToken ct)
        {
            await _authDbContext.Set<RefreshToken>().AddAsync(token, ct);
            await _authDbContext.SaveChangesAsync(ct);
        }

        public async Task RevokeRefreshTokenForUser(int userId, CancellationToken ct)
        {
            var existingRefreshToken = await _authDbContext.Set<RefreshToken>()
                .FirstOrDefaultAsync(r => r.UserId == userId && !r.IsRevoked, cancellationToken: ct);

            if (existingRefreshToken is null)
            {
                return;
            }

            existingRefreshToken.UpdateRevokeStatus(
               true,
               DateTime.UtcNow);

            await _authDbContext.SaveChangesAsync(ct);
        }
    }
}