using System.Text.Json.Serialization;

namespace Auth.MicroService.WebApi.Models
{
    public class PostResetPasswordModel
    {
        [JsonRequired]
        public string Token { get; init; }

        [JsonRequired]
        public string NewPassword { get; init; }
    }
}
