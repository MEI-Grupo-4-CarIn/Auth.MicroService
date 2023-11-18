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
    /// The authentication service
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
            var user = User.CreateNewUser(
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
            var user = await _userRepository.GetUserByEmail(model.Email, ct);

            if (user is not null)
            {
                var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

                if (passwordResult == PasswordVerificationResult.Success)
                {
                    var token = _jwtProvider.GenerateJwt(user);
                    return token;
                }
            }

            throw new UnauthorizedAccessException("Invalid login attempt.");
        }
    }
}
