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
        Task<TokenModel> UserLogin(LoginModel model, CancellationToken ct);

        /// <summary>
        /// Performs one user logout.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<int?> UserLogout(string token, CancellationToken ct);

        /// <summary>
        /// Generates a password reset token.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TokenModel> GeneratePasswordResetToken(string email, CancellationToken ct);

        /// <summary>
        /// Resets a user's password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> ResetPassword(ResetPasswordModel model, CancellationToken ct);

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
        Task<TokenModel> RefreshOneToken(string token, CancellationToken ct);
    }
}
