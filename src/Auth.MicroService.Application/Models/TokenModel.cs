namespace Auth.MicroService.Application.Models
{
    public record TokenModel(string Token, string RefreshToken, long ExpiresIn);
}