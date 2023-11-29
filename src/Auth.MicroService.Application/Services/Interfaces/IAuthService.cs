using Auth.MicroService.Application.Models;
using System.Threading.Tasks;
using System.Threading;

namespace Auth.MicroService.Application.Services.Interfaces
{
    /// <summary>
    /// The authentication service
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Performs the user registration.
        /// </summary>
        /// <param name="model">The register model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UserRegistration(RegisterModel model, CancellationToken ct);

        /// <summary>
        /// Performs one user login.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> UserLogin(LoginModel model, CancellationToken ct);

        /// <summary>
        /// Validates one token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="bool"/> whether the token is valid or not.</returns>
        bool ValidateToken(string token, CancellationToken ct);

        /// <summary>
        /// Refreshes one token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> RefreshToken(string token, CancellationToken ct);
    }
}
