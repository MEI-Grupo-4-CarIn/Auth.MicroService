﻿using Auth.MicroService.Domain.Entities;
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

        Task<IEnumerable<UserInfo>> GetAllInactiveUsers(CancellationToken ct);

        Task<string> UpdateUser(User user, CancellationToken ct);

        Task<IEnumerable<UserInfo>> GetAllUsers(CancellationToken ct);

        Task<UserInfo> GetUserInfoById(int id, CancellationToken ct);
    }
}
