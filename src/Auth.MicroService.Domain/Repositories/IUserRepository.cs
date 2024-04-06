using Auth.MicroService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Domain.Repositories
{
    public interface IUserRepository
    {
        Task AddNewUser(User user, CancellationToken ct);

        Task<User> GetUserByEmail(string email, CancellationToken ct);

        Task<User> GetUserById(int id, CancellationToken ct);

        Task<IEnumerable<UserInfo>> GetAllInactiveUsers(int page, int perPage, CancellationToken ct);

        Task<string> UpdateUser(User user, CancellationToken ct);

        Task<IEnumerable<UserInfo>> GetAllUsers(string search, int page, int perPage, CancellationToken ct);

        Task<UserInfo> GetUserInfoById(int id, CancellationToken ct);
    }
}
