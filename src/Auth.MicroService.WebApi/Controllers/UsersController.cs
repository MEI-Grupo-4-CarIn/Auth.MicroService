using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.WebApi.Mapping;
using Auth.MicroService.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        /// Gets the list of users wating for approval.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("waiting-for-approval-list")]
        public async Task<ActionResult> GetUsersForApproval(CancellationToken ct)
        {
            var usersList = await _usersService.GetAllUsersForApproval(ct);

            return Ok(usersList);
        }
    }
}
