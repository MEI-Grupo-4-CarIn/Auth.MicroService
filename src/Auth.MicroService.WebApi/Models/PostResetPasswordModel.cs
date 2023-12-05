using System.Text.Json.Serialization;

namespace Auth.MicroService.WebApi.Models
{
    public class PostResetPasswordModel
    {
        [JsonRequired]
        public string Token { get; set; }

        [JsonRequired]
        public string NewPassword { get; set; }
    }
}
