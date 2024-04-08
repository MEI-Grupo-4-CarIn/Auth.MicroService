using Auth.MicroService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddNewRefreshToken(RefreshToken token, CancellationToken ct);
        
        Task RevokeRefreshTokenForUser(int userId, CancellationToken ct);
    }
}
