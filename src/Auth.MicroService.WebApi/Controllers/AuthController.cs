using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.WebApi.Mapping;
using Auth.MicroService.WebApi.Models;
using Auth.MicroService.WebApi.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Auth.MicroService.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

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
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        /// <param name="emailService">The email service.</param>
        public AuthController(IAuthService authService,
            IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The user registration model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserInfoResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserInfoResponseModel>> Register(PostRegisterModel model, CancellationToken ct)
        {
            var registerModel = UserMapper.PostRegisterModelToRegisterModel(model);
            try
            {
                var user = await _authService.UserRegistration(registerModel, ct);

                Log.Information("User '{Email}' has registered successfully.", registerModel.Email);
                return CreatedAtAction(
                    "GetUserById",
                    "Users",
                    new { id = user.UserId },
                    user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while registering the user.");
                return BadRequest(new ErrorResponseModel
                {
                    Error = "An error occurred while registering the user.",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Logins a user.
        /// </summary>
        /// <param name="model">The user login model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenModel>> Login(PostLoginModel model, CancellationToken ct)
        {
            var loginModel = UserMapper.PostLoginModelToLoginModel(model);
            try
            {
                var tokenModel = await _authService.UserLogin(loginModel, ct);

                Log.Information("User '{Email}' has logging in successfully.", loginModel.Email);

                return Ok(tokenModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while logging the user '{Email}' in.", loginModel.Email);
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"An error occurred while logging the user '{loginModel.Email}' in.",
                    Message = ex.Message
                });
            }
        }
        
        /// <summary>
        /// Logouts a user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin, Manager, Driver")]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Logout(PostLogoutModel model, CancellationToken ct)
        {
            var logoutModel = UserMapper.PostLogoutModelToLogoutModel(model);
            try
            {
                var token = GetTokenFromHeader();
                var userId = await _authService.UserLogout(logoutModel, token, ct);

                if (userId is null)
                {
                    return Ok();
                }
                
                Log.Information("User '{Id}' has logged out successfully.", userId.Value);
                return Ok("Logged out successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while logging the user out.");
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"An error occurred while logging the user out.",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Allows the user to request a password reset.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("forgotPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ForgotPassword(PostForgotPasswordModel model, CancellationToken ct)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(model);
                
                var tokenModel = await _authService.GeneratePasswordResetToken(model.Email, ct);
                await _emailService.SendPasswordResetEmail(model.Email, tokenModel.Token, ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the password reset request for email '{Email}'.", model.Email);
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"An error occurred while processing the password reset request for email '{model.Email}'.",
                    Message = ex.Message
                });
            }

            Log.Information("Password reset token generated and email sent to '{Email}'.", model.Email);

            return Ok($"Password reset token generated and email sent to '{model.Email}'.");
        }

        /// <summary>
        /// Performs a password reset.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("resetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ResetPassword(PostResetPasswordModel model, CancellationToken ct)
        {
            string email;

            try
            {
                ArgumentNullException.ThrowIfNull(model);
                
                var resetPasswordModel = UserMapper.PostResetPasswordModelToResetPasswordModel(model);

                email = await _authService.ResetPassword(resetPasswordModel, ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while resetting the password.");
                return BadRequest(new ErrorResponseModel
                {
                    Error = "An error occurred while resetting the password.",
                    Message = ex.Message
                });
            }

            Log.Information("Password has been reset successfully for user '{Email}'.", email);
            return Ok($"Password has been reset successfully for user '{email}'.");
        }

        /// <summary>
        /// Validates one token.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("validateToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public ActionResult ValidateToken(PostValidateTokenModel model, CancellationToken ct)
        {
            var isValid = _authService.ValidateToken(model.Token, ct);
            if (!isValid)
            {
                return BadRequest(new ErrorResponseModel
                {
                    Error = "An error occurred while sending the request.",
                    Message = "Invalid token."
                });
            }
            
            Log.Information("A token has been successfully validated.");
            return Ok(new { Message = "Token is valid." });
        }

        /// <summary>
        /// Refreshes one token.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("refreshToken")]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenModel>> RefreshToken(PostRefreshTokenModel model, CancellationToken ct)
        {
            try
            {
                var newTokenModel = await _authService.RefreshOneToken(model.RefreshToken, ct);
                return Ok(newTokenModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while refreshing the token.");
                return BadRequest(new ErrorResponseModel
                {
                    Error = "An error occurred while refreshing the token.",
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
