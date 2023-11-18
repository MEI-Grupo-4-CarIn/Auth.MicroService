using Auth.MicroService.Domain.Entities;

namespace Auth.MicroService.Application.JwtUtils
{
    public interface IJwtProvider
    {
        string GenerateJwt(User user);
    }
}
