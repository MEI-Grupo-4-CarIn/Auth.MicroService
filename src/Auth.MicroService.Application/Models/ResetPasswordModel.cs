namespace Auth.MicroService.Application.Models
{
    public class ResetPasswordModel
    {
        public string Token { get; set; }

        public string NewPassword { get; set; }
    }
}
