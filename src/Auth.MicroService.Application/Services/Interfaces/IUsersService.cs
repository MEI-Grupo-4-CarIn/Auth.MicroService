using Auth.MicroService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Auth.MicroService.Application.Models;

namespace Auth.MicroService.Application.Services.Interfaces
{
    /// <summary>
    /// The users service.
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Gets a list with the users waiting for approval.
        /// </summary>
        /// <param name="page">The page to be requested.</param>
        /// <param name="perPage">The amount of items requested per page.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of users.</returns>
        Task<IEnumerable<UserInfo>> GetAllUsersForApproval(int page, int perPage, CancellationToken ct);

        /// <summary>
        /// Performs the approval of a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="roleId">The role id.</param>
        /// <param name="token">The jwt token.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        Task ApproveUser(int id, int? roleId, string token, CancellationToken ct);

        /// <summary>
        /// Performs the update of a user's information.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="model">The model.</param>
        /// <param name="token">The jwt token.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The user email.</returns>
        Task<string> UpdateUserInfo(int id, UpdateUserModel model, string token, CancellationToken ct);

        /// <summary>
        /// Performs the update of a user's password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="token">The jwt token.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The user email.</returns>
        Task<string> ChangeUserPassword(ChangePasswordModel model, string token, CancellationToken ct);

        /// <summary>
        /// Performs the inactivation of a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="token">The jwt token.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        Task DeleteUser(int id, string token, CancellationToken ct);

        /// <summary>
        /// Gets a list with the users.
        /// </summary>
        /// <param name="search">The search text.</param>
        /// <param name="page">The page to be requested.</param>
        /// <param name="perPage">The amount of items requested per page.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of users.</returns>
        Task<IEnumerable<UserInfo>> GetAllUsers(string search, int page, int perPage, CancellationToken ct);

        /// <summary>
        /// Gets a specific user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The user info.</returns>
        Task<UserInfo> GetUserById(int id, CancellationToken ct);
    }
}
