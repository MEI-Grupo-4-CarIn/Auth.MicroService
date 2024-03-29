﻿using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.WebApi.Mapping;
using Auth.MicroService.WebApi.Models;
using Auth.MicroService.WebApi.Models.Response;
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
                return BadRequest(new ErrorResponseModel
                {
                    Error = "An error occurred while registering the user.",
                    Message = ex.Message
                });
            }

            Log.Information("User '{Email}' has registered successfully.", registerModel.Email);

            return Ok($"User '{registerModel.Email}' has registered successfully.");
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
                Log.Error(ex, "An error occurred while logging the user '{Email}' in.", loginModel.Email);
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"An error occurred while logging the user '{loginModel.Email}' in.",
                    Message = ex.Message
                });
            }

            Log.Information("User '{Email}' has logging in successfully.", loginModel.Email);

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Allows the user to request a password reset.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("forgotPassword")]
        public async Task<ActionResult> ForgotPassword(PostForgotPasswordModel model, CancellationToken ct)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            try
            {
                var token = await _authService.GeneratePasswordResetToken(model.Email, ct);
                await _emailService.SendPasswordResetEmail(model.Email, token, ct);
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
        public async Task<ActionResult> ResetPassword(PostResetPasswordModel model, CancellationToken ct)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            string email;

            try
            {
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
        /// <param name="token">The token.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("validateToken")]
        public ActionResult ValidateToken(string token, CancellationToken ct)
        {
            var isValid = _authService.ValidateToken(token, ct);

            if (isValid)
            {
                Log.Information("A token has been successfully validated.");
                return Ok(new { Message = "Token is valid." });
            }
            else
            {
                return BadRequest(new ErrorResponseModel
                {
                    Error = $"An error occurred while sending the request.",
                    Message = "Invalid token"
                });
            }
        }

        /// <summary>
        /// Refreshes one token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("refreshToken")]
        public async Task<ActionResult<string>> RefreshToken(string token, CancellationToken ct)
        {
            var newToken = await _authService.RefreshToken(token, ct);

            if (newToken is not null)
            {
                return Ok(new { Token = newToken });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
