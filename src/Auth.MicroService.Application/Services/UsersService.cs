using Auth.MicroService.Application.JwtUtils;
using Auth.MicroService.Application.Models;
using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Enums;
using Auth.MicroService.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Auth.MicroService.Application.Mapping;

namespace Auth.MicroService.Application.Services
{
    /// <summary>
    /// The users service.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="passwordHasher">The password hasher.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="jwtProvider">The jwt provider.</param>
        public UsersService(
            IPasswordHasher<User> passwordHasher,
            IUserRepository userRepository,
            IJwtProvider jwtProvider)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserInfoResponseModel>> GetUsersForApprovalList(int page, int perPage, CancellationToken ct)
        {
            var users = await _userRepository.GetInactiveUsersList(page, perPage, ct);

            return UserMapper.UserInfoToUserInfoResponseModel(users);
        }

        /// <inheritdoc/>
        public async Task ApproveUser(int id, int? roleId, string token, CancellationToken ct)
        {
            var user = await _userRepository.GetUserById(id, ct);

            if (user is null)
            {
                throw new Exception("User not found.");
            }

            if (user.Status)
            {
                throw new Exception("User already active.");
            }

            // Get user role from token
            var userRole = _jwtProvider.GetUserRoleFromToken(token);
            if (userRole is null)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            Role? roleEnum = null;
            if (roleId.HasValue)
            {
                roleEnum = (Role)roleId.Value;
                if (!Enum.IsDefined(typeof(Role), roleEnum))
                {
                    throw new InvalidEnumArgumentException($"The given value '{roleId}' is not valid for Role definition.");
                }
                // Validate user roles hierarchy
                CheckUserHierarchy(userRole.Value, roleEnum.Value);
            }
            
            var updatedUser = User.SetUserActivation(
                user,
                roleEnum,
                status: true);

            await _userRepository.UpdateUser(updatedUser, ct);
        }

        /// <inheritdoc/>
        public async Task<string> UpdateUserInfo(int id, UpdateUserModel model, string token, CancellationToken ct)
        {
            var userId = _jwtProvider.GetUserIdFromToken(token);
            if (userId is null)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            var userRole = _jwtProvider.GetUserRoleFromToken(token);
            if (userRole is null)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }
            
            if (userId != id && userRole != Role.Admin)
            {
                throw new UnauthorizedAccessException("You have no permissions to update other user's information.");
            }

            var user = await _userRepository.GetUserById(userId.Value, ct);
            if (user is null)
            {
                throw new Exception("User not found.");
            }

            var updatedUser = User.CreateNewUser(
                user.UserId,
                model.FirstName ?? user.FirstName,
                model.LastName ?? user.LastName,
                model.Email ?? user.Email,
                user.Password,
                user.BirthDate,
                user.RoleId,
                user.Status
            );

            if (!string.IsNullOrEmpty(model.Email) && !string.Equals(model.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var userByEmail = await _userRepository.GetUserByEmail(model.Email, ct);
                if (userByEmail is not null)
                {
                    throw new ArgumentException("Email already used.");
                }
            }

            return await _userRepository.UpdateUser(updatedUser, ct);
        }

        /// <inheritdoc/>
        public async Task<string> ChangeUserPassword(ChangePasswordModel model, string token, CancellationToken ct)
        {
            var userId = _jwtProvider.GetUserIdFromToken(token);
            if (userId is null)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            var user = await _userRepository.GetUserById(userId.Value, ct);
            if (user is null)
            {
                throw new Exception("User not found.");
            }

            // Verify the old password
            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.OldPassword);
            if (passwordResult != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid old password.");
            }

            if (!string.Equals(model.NewPassword, model.ConfirmNewPassword))
            {
                throw new ArgumentException("NewPassword and ConfirmNewPassword are not equals.");
            }

            var userForValidation = User.CreateNewUser(
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                model.NewPassword,
                user.BirthDate,
                user.RoleId,
                user.Status
            );

            var userToUpdate = User.SetUserHashedPassword(user, _passwordHasher.HashPassword(userForValidation, userForValidation.Password));

            return await _userRepository.UpdateUser(userToUpdate, ct);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<IEnumerable<UserInfoResponseModel>> GetUsersList(
            string search,
            Role? role,
            int page,
            int perPage,
            CancellationToken ct)
        {
            var users = await _userRepository.GetUsersList(search, role, page, perPage, ct);

            return UserMapper.UserInfoToUserInfoResponseModel(users);
        }

        /// <inheritdoc/>
        public async Task<UserInfoResponseModel> GetUserById(int id, CancellationToken ct)
        {
            var user = await _userRepository.GetUserInfoById(id, ct);
            if(user is null)
            {
                throw new Exception("User not found.");
            }

            return new UserInfoResponseModel(
                user.UserId,
                user.UserFullName,
                user.Email,
                user.Role,
                user.Status,
                user.CreationDate,
                user.LastUpdateDate);
        }

        private static void CheckUserHierarchy(Role userRole, Role roleToApply, bool isDelete = false)
        {
            if ((int)roleToApply >= (int)userRole) return;
            
            if (isDelete)
            {
                throw new Exception("You cannot deactivate a user with higher role than yours.");
            }

            throw new Exception("You cannot assign a role that is higher than yours.");
        }
    }
}
