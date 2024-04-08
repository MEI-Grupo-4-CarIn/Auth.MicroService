using System;
using Auth.MicroService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddNewRefreshToken(RefreshToken token, CancellationToken ct);
        Task RevokeRefreshTokenForUser(int userId, string refreshToken, CancellationToken ct);
        Task UpdateExpiresIn(int refreshTokenId, DateTime dateTime, CancellationToken ct);
        Task<RefreshToken> GetRefreshToken(string refreshToken, CancellationToken ct);
    }
}
