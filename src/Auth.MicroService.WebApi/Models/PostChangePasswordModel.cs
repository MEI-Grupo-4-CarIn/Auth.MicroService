using System.ComponentModel.DataAnnotations;

namespace Auth.MicroService.WebApi.Models
{
    public class PostChangePasswordModel
    {
        [Required]
        public string OldPassword { get; init; }

        [Required]
        public string NewPassword { get; init; }

        [Required]
        public string ConfirmNewPassword { get; init; }
    }
}
