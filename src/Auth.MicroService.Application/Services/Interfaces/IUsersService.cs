using Auth.MicroService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Auth.MicroService.Application.Models;

namespace Auth.MicroService.Application.Services.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<UserInfo>> GetAllUsersForApproval(int page, int perPage, CancellationToken ct);

        Task ApproveUser(int id, int? roleId, string token, CancellationToken ct);

        Task<string> UpdateUserInfo(int id, UpdateUserModel model, string token, CancellationToken ct);

        Task<string> ChangeUserPassword(ChangePasswordModel model, string token, CancellationToken ct);

        Task DeleteUser(int id, string token, CancellationToken ct);

        Task<IEnumerable<UserInfo>> GetAllUsers(string search, int page, int perPage, CancellationToken ct);

        Task<UserInfo> GetUserById(int id, CancellationToken ct);
    }
}
