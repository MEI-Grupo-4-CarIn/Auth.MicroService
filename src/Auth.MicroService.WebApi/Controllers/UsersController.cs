using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.WebApi.Mapping;
using Auth.MicroService.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The user registration model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("waiting-for-approval-list")]
        public async Task<ActionResult> GetUsersForApproval(CancellationToken ct)
        {
            var usersList = await _usersService.GetAllUsersForApproval(ct);

            return Ok(usersList);
        }

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("delete-user")]
        public async Task<ActionResult> DeleteUser(int id, CancellationToken ct)
        {
            bool result = await _usersService.DeleteUser(id, ct);

            if (result == false)
            {
                Log.Error($"An error occurred while deactivating the user with id: {id}.");
                return StatusCode(500, $"An error occurred while deactivating the user with id: {id}.");
            }

            return Ok("User deactivated.");
        }
    }
}
