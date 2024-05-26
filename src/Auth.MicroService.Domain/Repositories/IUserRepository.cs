using Auth.MicroService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Auth.MicroService.Domain.Enums;

namespace Auth.MicroService.Domain.Repositories
{
    public interface IUserRepository
    {
        Task AddNewUser(User user, CancellationToken ct);

        Task<User> GetUserByEmail(string email, CancellationToken ct);

        Task<User> GetUserById(int id, CancellationToken ct);

        Task<IEnumerable<User>> GetInactiveUsersList(int page, int perPage, CancellationToken ct);

        Task<string> UpdateUser(User user, CancellationToken ct);

        Task<IEnumerable<User>> GetUsersList(
            string search,
            Role? role,
            int page,
            int perPage,
            CancellationToken ct);
    }
}
