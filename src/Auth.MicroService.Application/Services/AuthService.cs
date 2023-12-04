using Auth.MicroService.Application.JwtUtils;
using Auth.MicroService.Application.Models;
using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Application.Services
{
    /// <summary>
    /// The authentication service.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        /// <inheritdoc/>
        public async Task UserRegistration(RegisterModel model, CancellationToken ct)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var userByEmail = await _userRepository.GetUserByEmail(model.Email, ct);
            if (userByEmail is not null)
            {
                throw new ArgumentException("Email already used.");
            }

            var user = User.CreateNewUser(
                userId: null,
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password,
                model.BirthDate);

            var userToInsert = User.SetUserHashedPassword(user, _passwordHasher.HashPassword(user, user.Password));

            await _userRepository.AddNewUser(userToInsert, ct);
        }

        /// <inheritdoc/>
        public async Task<string> UserLogin(LoginModel model, CancellationToken ct)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var user = await _userRepository.GetUserByEmail(model.Email, ct);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            // Verify the password
            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (passwordResult != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            if (user.Status == false)
            {
                throw new UnauthorizedAccessException("Inactive user.");
            }

            var token = _jwtProvider.GenerateJwt(user);
            return token;
        }

        /// <inheritdoc/>
        public bool ValidateToken(string token, CancellationToken ct)
        {
            return _jwtProvider.ValidateToken(token);
        }

        /// <inheritdoc/>
        public async Task<string> RefreshToken(string token, CancellationToken ct)
        {
            var isValid = ValidateToken(token, ct);

            if (!isValid)
            {
                return null;
            }

            var userId = _jwtProvider.GetUserIdFromToken(token);

            if (userId is null)
            {
                return null;
            }

            var user = await _userRepository.GetUserById(userId.Value, ct);

            if (user is null)
            {
                return null;
            }

            // Generate a new token
            var newToken = _jwtProvider.GenerateJwt(user);
            return newToken;
        }

    }
}
