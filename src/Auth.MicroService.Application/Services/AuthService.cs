using Auth.MicroService.Application.Models;
using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Application.Services
{
    /// <summary>
    /// The authentication service
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher<RegisterModel> _passwordHasher;
        private readonly IUserRepository _userRepository;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<RegisterModel> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        /// <inheritdoc/>
        public async Task UserRegistration(RegisterModel model, CancellationToken ct)
        {
            var user = User.CreateNewUser(
                model.FirstName,
                model.LastName,
                model.Email,
                _passwordHasher.HashPassword(model, model.Password),
                model.BirthDate);

            await _userRepository.AddNewUser(user, ct);
        }
    }
}
