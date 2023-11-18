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
    }
}
