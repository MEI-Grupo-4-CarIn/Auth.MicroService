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

        public async Task RevokeRefreshTokenForUser(int userId, string refreshToken, CancellationToken ct)
        {
            var existingRefreshToken = await _authDbContext.Set<RefreshToken>()
                .SingleOrDefaultAsync(r => r.Token.Equals(refreshToken)
                                           && r.UserId == userId
                                           && !r.IsRevoked, ct);

            if (existingRefreshToken is null)
            {
                return;
            }

            existingRefreshToken.UpdateRevokeStatus(
               true,
               DateTime.UtcNow);

            await _authDbContext.SaveChangesAsync(ct);
        }

        public async Task UpdateExpiresIn(int refreshTokenId, DateTime dateTime, CancellationToken ct)
        {
            var existingRefreshToken = await _authDbContext.Set<RefreshToken>()
                .SingleOrDefaultAsync(r => r.RefreshTokenId == refreshTokenId, ct);

            if (existingRefreshToken is null)
            {
                return;
            }

            existingRefreshToken.UpdateExpiresIn(dateTime);

            await _authDbContext.SaveChangesAsync(ct);
        }

        public async Task<RefreshToken> GetRefreshToken(string refreshToken, CancellationToken ct)
        {
            return await _authDbContext.Set<RefreshToken>()
                .AsNoTracking()
                .SingleOrDefaultAsync(r => r.Token.Equals(refreshToken), ct);
        }
    }
}