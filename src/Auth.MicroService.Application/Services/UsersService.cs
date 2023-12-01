using Auth.MicroService.Application.JwtUtils;
using Auth.MicroService.Application.Models;
using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Enums;
using Auth.MicroService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Application.Services
{
    /// <summary>
    /// The users service.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public UsersService(
            IUserRepository userRepository,
            IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<IEnumerable<UserInfo>> GetAllUsersForApproval(CancellationToken ct)
        {
            return await _userRepository.GetAllInactiveUsers(ct);
        }

        public async Task ApproveUser(ApproveUserModel model, string token, CancellationToken ct)
        {
            var user = await _userRepository.GetUserById(model.Id, ct);

            if (user is null)
            {
                throw new Exception("User not found.");
            }

            if (user.Status == true)
            {
                throw new Exception("User already active.");
            }

            // Get user role from token
            var userRole = _jwtProvider.GetUserRoleFromToken(token);
            if (userRole is null)
            {
                throw new UnauthorizedAccessException();
            }

            if (model.Role.HasValue)
            {
                // Validate user roles hierarchy
                CheckUserHierarchy(userRole.Value, model.Role.Value);
            }

            var updatedUser = User.SetUserActivation(
                user,
                model.Role.HasValue ? model.Role.Value : null,
                status: true);

            await _userRepository.UpdateUser(updatedUser, ct);
        }

        public async Task DeleteUser(int id, string token, CancellationToken ct)
        {
            var user = await _userRepository.GetUserById(id, ct);

            if(user is null)
            {
                throw new Exception("User not found.");
            }

            var userRole = _jwtProvider.GetUserRoleFromToken(token);
            if (userRole is null)
            {
                throw new UnauthorizedAccessException();
            }

            CheckUserHierarchy(userRole.Value, user.RoleId, isDelete: true);

            var deactivatedUser = User.SetUserActivation(user, null, status: false);

            await _userRepository.UpdateUser(deactivatedUser, ct);
        }

        public async Task<IEnumerable<UserInfo>> GetAllUsers(CancellationToken ct)
        {
            return await _userRepository.GetAllUsers(ct);
        }

        private void CheckUserHierarchy(Role userRole, Role roleToApply, bool isDelete = false)
        {
            if ((int)roleToApply < (int)userRole)
            {
                if (isDelete)
                {
                    throw new Exception("You cannot deactivate a user with higher role than yours.");
                }

                throw new Exception("You cannot assign a role that is higher than yours.");
            }
        }
    }
}
