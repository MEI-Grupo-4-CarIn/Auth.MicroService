using Auth.MicroService.Domain.Enums;

namespace Auth.MicroService.Application.Models
{
    public record AuthResponseModel(
        int UserId,
        string Email,
        string FirstName,
        string LastName,
        Role Role,
        string Token,
        string RefreshToken,
        long ExpiresIn)
        : TokenModel(
            Token,
            RefreshToken,
            ExpiresIn);
}