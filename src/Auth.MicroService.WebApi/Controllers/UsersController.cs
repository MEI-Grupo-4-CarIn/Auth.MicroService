﻿using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.WebApi.Mapping;
using Auth.MicroService.WebApi.Models;
using Auth.MicroService.WebApi.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.WebApi.Controllers
{
    /// <summary>
    /// This controller handles users management actions.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="usersService">The users service.</param>
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Gets the list of users waiting for approval.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("waiting-for-approval-list")]
        public async Task<ActionResult> GetUsersForApproval(CancellationToken ct)
        {
            var usersList = await _usersService.GetAllUsersForApproval(ct);

            return Ok(usersList.Any() ? usersList : "No users waiting for approval.");
        }

        /// <summary>
        /// Allows the administrator or manager to approve one user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost("approve-user")]
        public async Task<ActionResult> ApproveUser(PostApproveUserModel model, CancellationToken ct)
        {
            try
            {
                var approveUserModel = UserMapper.PostApproveUserModelToApproveUserModel(model);

                string token = GetTokenFromHeader();

                await _usersService.ApproveUser(approveUserModel, token, ct);

                Log.Information($"User with id '{model.Id}' approved with success.");
                return Ok($"User with id '{model.Id}' approved with success.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while approving user with id '{model.Id}'.");
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"Error while approving user with id '{model.Id}'.",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Allows every user to update their information.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin, Manager, Driver")]
        [HttpPatch("update-user-info")]
        public async Task<ActionResult> UpdateUserInfo(PatchUpdateUserModel model, CancellationToken ct)
        {
            try
            {
                var updateUserModel = UserMapper.PatchUpdateUserModelToUpdateUserModel(model);

                // Get the token to identify the user and then update it's own information
                string token = GetTokenFromHeader();

                string email = await _usersService.UpdateUserInfo(updateUserModel, token, ct);

                Log.Information($"User with email '{email}' updated with success.");
                return Ok($"User with email '{email}' updated with success.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while updating user information.");
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"Error while updating user information.",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Allows every authenticated user to change their password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin, Manager, Driver")]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(PostChangePasswordModel model, CancellationToken ct)
        {
            try
            {
                var changePasswordModel = UserMapper.PostChangePasswordModelToChangePasswordModel(model);

                // Get the token to identify the user and then update it's own information
                string token = GetTokenFromHeader();

                string email = await _usersService.ChangeUserPassword(changePasswordModel, token, ct);

                Log.Information($"Password from user '{email}' updated with success.");
                return Ok($"Password from user '{email}' updated with success.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while updating user password.");
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"Error while updating user password.",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin, Manager")]
        [HttpDelete("delete-user")]
        public async Task<ActionResult> DeleteUser(int id, CancellationToken ct)
        {
            try
            {
                string token = GetTokenFromHeader();

                await _usersService.DeleteUser(id, token, ct);

                return Ok($"User with id '{id}' deactivated.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deactivating the user with id '{id}'.", id);
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"An error occurred while deactivating the user with id '{id}'.",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// List all users.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpGet("all-users-list")]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetAllUsers(CancellationToken ct)
        {
            var usersList = await _usersService.GetAllUsers(ct);
            return Ok(usersList.Any() ? usersList : "No users to show.");
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpGet("get-by-id")]
        public async Task<ActionResult<UserInfo>> GetUserById(int id, CancellationToken ct)
        {
            try
            {
                var user = await _usersService.GetUserById(id, ct);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"An error occurred while getting the user with id '{id}'.",
                    Message = ex.Message
                });
            }
        }

        private string GetTokenFromHeader()
        {
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Split(' ')[1]; // Bearer <token>
                return token;
            }
            else
            {
                throw new Exception("Authorization header not found");
            }
        }
    }
}
