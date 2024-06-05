using Auth.MicroService.Application.Services.Interfaces;
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
using Auth.MicroService.Application.Models;
using Auth.MicroService.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Auth.MicroService.WebApi.Controllers
{
    /// <summary>
    /// This controller handles users management actions.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private const int MaxPerPage = 100;
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
        /// Gets a list of users.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="search">The search text.</param>
        /// <param name="role">The role filter.</param>
        /// <param name="page">The page to be requested.</param>
        /// <param name="perPage">The amount of items requested per page.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserInfoResponseModel>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<UserInfoResponseModel>>> GetUsersList(CancellationToken ct,
            [FromQuery] string search,
            [FromQuery] Role? role,
            [FromQuery] int perPage = 10,
            [FromQuery] int page = 1)
        {
            try
            {
                if (perPage > MaxPerPage)
                {
                    throw new ArgumentOutOfRangeException(nameof(perPage), $"The maximum number of items per page is {MaxPerPage}.");
                }
                
                var usersList = await _usersService.GetUsersList(search, role, page, perPage, ct);
                if (usersList.Any())
                {
                    return Ok(usersList);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseModel
                {
                    Error = "An error occurred while retrieving users.",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets the list of users waiting for approval.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="page">The page to be requested.</param>
        /// <param name="perPage">The amount of items requested per page.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "1, 2")]
        [HttpGet("waiting-for-approval")]
        [ProducesResponseType(typeof(IEnumerable<UserInfoResponseModel>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<UserInfoResponseModel>>> GetUsersForApproval(CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10)
        {
            try
            {
                if (perPage > MaxPerPage)
                {
                    throw new ArgumentOutOfRangeException(nameof(perPage), $"The maximum number of items per page is {MaxPerPage}.");
                }
            
                var usersList = await _usersService.GetUsersForApprovalList(page, perPage, ct);
                if (usersList.Any())
                {
                    return Ok(usersList);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseModel
                {
                    Error = "An error occurred while retrieving users.",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Allows the administrator or manager to approve one user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="roleId">The role to apply.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "1, 2")]
        [HttpPost("{id:int}/approval")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ApproveUser(int id, CancellationToken ct,
            [FromQuery] int? roleId)
        {
            try
            {
                string token = GetTokenFromHeader();

                await _usersService.ApproveUser(id, roleId, token, ct);

                Log.Information($"User with id '{id}' approved with success.");
                return Ok($"User with id '{id}' approved with success.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while approving user with id '{id}'.");
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"Error while approving user with id '{id}'.",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates an user information.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "1, 2, 3")]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateUserInfo(int id,
            PatchUpdateUserModel model,
            CancellationToken ct)
        {
            try
            {
                var updateUserModel = UserMapper.PatchUpdateUserModelToUpdateUserModel(model);

                // Get the token to identify the user
                string token = GetTokenFromHeader();
                string email = await _usersService.UpdateUserInfo(id, updateUserModel, token, ct);

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
        [Authorize(Roles = "1, 2, 3")]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
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
        /// Deletes a user by id.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "1, 2")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteUserById(int id, CancellationToken ct)
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
        /// Gets a user by id.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserInfoResponseModel),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserInfoResponseModel>> GetUserById(int id, CancellationToken ct)
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
