namespace Auth.MicroService.Application.Models
{
    public class ResetPasswordModel
    {
        public string Token { get; init; }
        public string NewPassword { get; init; }
    }
}
