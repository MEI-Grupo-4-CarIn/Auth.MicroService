namespace Auth.MicroService.Application.Models
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; init; }
        public string NewPassword { get; init; }
        public string ConfirmNewPassword { get; init; }
    }
}
