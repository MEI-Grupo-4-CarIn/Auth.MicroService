using System.Text.Json.Serialization;

namespace Auth.MicroService.WebApi.Models
{
    public class PostForgotPasswordModel
    {
        [JsonRequired]
        public string Email { get; init; }
    }
}
