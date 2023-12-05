using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetEmail(string email, string token, CancellationToken ct);
    }
}
