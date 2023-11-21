using Auth.MicroService.Application.Models;
using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.WebApi.Mapping;
using Auth.MicroService.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.WebApi.Controllers
{
    /// <summary>
    /// This controller handles authentication-related actions.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The user registration model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("register")]
        public async Task<ActionResult> Register(PostRegisterModel model, CancellationToken ct)
        {
            var registerModel = UserMapper.PostRegisterModelToRegisterModel(model);

            try
            {
                await _authService.UserRegistration(registerModel, ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while registering the user.");
                return StatusCode(500, "An error occurred while registering the user.");
            }

            Log.Information("User {Email} has registered successfully.", registerModel.Email);

            return Ok();
        }

        /// <summary>
        /// Logins a user.
        /// </summary>
        /// <param name="model">The user login model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(PostLoginModel model, CancellationToken ct)
        {
            var loginModel = UserMapper.PostLoginModelToLoginModel(model);
            string token;

            try
            {
                token = await _authService.UserLogin(loginModel, ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while logging the user {Email} in.", loginModel.Email);
                return StatusCode(500, "An error occurred while logging the user in.");
            }

            Log.Information("User {Email} has logging in successfully.", loginModel.Email);

            return Ok(token);
        }
    }
}
