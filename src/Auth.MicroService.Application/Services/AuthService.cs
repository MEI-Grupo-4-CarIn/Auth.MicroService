using Auth.MicroService.Application.JwtUtils;
using Auth.MicroService.Application.Models;
using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMemoryCache _cache;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher<User> passwordHasher,
            IJwtProvider jwtProvider,
            IMemoryCache cache)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _cache = cache;
        }

        /// <inheritdoc/>
        public async Task UserRegistration(RegisterModel model, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(model);

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
        public async Task<TokenModel> UserLogin(LoginModel model, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await _userRepository.GetUserByEmail(model.Email, ct);
            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            // Verify the password
            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (passwordResult != PasswordVerificationResult.Success)
            {
                IncrementFailedLoginAttempt(model.Email);
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            if (user.Status == false)
            {
                throw new UnauthorizedAccessException("Inactive user.");
            }

            var tokenModel = _jwtProvider.GenerateJwt(user);
            var refreshToken = RefreshToken.CreateNewRefreshToken(
                null,
                user.UserId.Value,
                tokenModel.RefreshToken,
                DateTime.UtcNow.AddDays(30));
            
            await _refreshTokenRepository.AddNewRefreshToken(refreshToken, ct);
            
            ResetFailedLoginAttempt(model.Email);
            
            return tokenModel;
        }

        public async Task<int?> UserLogout(LogoutModel model, string token, CancellationToken ct)
        {
            var userId = _jwtProvider.GetUserIdFromToken(token);
            if (userId is null)
            {
                return null;
            }

            await _refreshTokenRepository.RevokeRefreshTokenForUser(userId.Value, model.RefreshToken, ct);
            return userId.Value;
        }

        /// <inheritdoc/>
        public async Task<TokenModel> GeneratePasswordResetToken(string email, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(email);

            var user = await _userRepository.GetUserByEmail(email, ct);
            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid email.");
            }

            var tokenModel = _jwtProvider.GeneratePasswordResetToken(user);
            return tokenModel;
        }

        /// <inheritdoc/>
        public async Task<string> ResetPassword(ResetPasswordModel model, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(model);

            var email = _jwtProvider.ValidatePasswordResetToken(model.Token);
            if (email is null)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            var userByEmail = await _userRepository.GetUserByEmail(email, ct);
            if (userByEmail is null)
            {
                throw new UnauthorizedAccessException("Invalid email.");
            }

            var user = User.CreateNewUser(
                userByEmail.UserId,
                userByEmail.FirstName,
                userByEmail.LastName,
                userByEmail.Email,
                model.NewPassword,
                userByEmail.BirthDate,
                userByEmail.RoleId,
                userByEmail.Status);

            // Reset the password
            var userToUpdate = User.SetUserHashedPassword(user, _passwordHasher.HashPassword(user, user.Password));
            
            return await _userRepository.UpdateUser(userToUpdate, ct);
        }

        /// <inheritdoc/>
        public bool ValidateToken(string token, CancellationToken ct)
        {
            return _jwtProvider.ValidateToken(token);
        }

        /// <inheritdoc/>
        public async Task<TokenModel> RefreshOneToken(string refreshToken, CancellationToken ct)
        {
            var refreshTokenEntity = await _refreshTokenRepository.GetRefreshToken(refreshToken, ct);
            if (refreshTokenEntity is null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var user = await _userRepository.GetUserById(refreshTokenEntity.UserId, ct);
            if (user is null)
            {
                throw new Exception("User not found.");
            }

            // Generate a new token without refresh token
            var tokenModel = _jwtProvider.GenerateJwt(user, false);
            
            // Add more 30 days to the existing refresh token
            await _refreshTokenRepository.UpdateExpiresIn(
                refreshTokenEntity.RefreshTokenId.Value,
                DateTime.UtcNow.AddDays(30),
                ct);
            
            return new TokenModel
            {
                Token = tokenModel.RefreshToken,
                ExpiresIn = tokenModel.ExpiresIn,
                RefreshToken = refreshTokenEntity.Token
            };
        }

        private void IncrementFailedLoginAttempt(string email)
        {
            var cacheEntry = _cache.GetOrCreate(email, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return new Tuple<int, DateTimeOffset>(0, DateTimeOffset.Now);
            });

            var updatedCacheEntry = new Tuple<int, DateTimeOffset>(cacheEntry.Item1 + 1, cacheEntry.Item2);
            _cache.Set(email, updatedCacheEntry, DateTimeOffset.Now.AddMinutes(5));

            if (updatedCacheEntry.Item1 > 3 && DateTimeOffset.Now - updatedCacheEntry.Item2 < TimeSpan.FromMinutes(5))
            {
                throw new UnauthorizedAccessException("Too many failed login attempts. Please try again later.");
            }
        }

        private void ResetFailedLoginAttempt(string email)
        {
            _cache.Remove(email);
        }
    }
}
