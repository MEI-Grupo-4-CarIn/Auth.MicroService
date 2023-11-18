using Auth.MicroService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Domain.Repositories
{
    public interface IUserRepository
    {
        Task AddNewUser(User user, CancellationToken ct);

        Task<User> GetUserByEmail(string email, CancellationToken ct);
    }
}
