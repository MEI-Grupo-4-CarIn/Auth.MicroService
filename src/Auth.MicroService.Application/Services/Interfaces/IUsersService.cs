﻿using Auth.MicroService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Auth.MicroService.Application.Services.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<UserInfo>> GetAllUsersForApproval(CancellationToken ct);

        Task<bool> DeleteUser(int id, CancellationToken ct);

        Task<IEnumerable<UserInfo>> GetAllUsers(CancellationToken ct);
    }
}
