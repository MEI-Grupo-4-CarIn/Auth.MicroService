using System.Text.Json.Serialization;

namespace Auth.MicroService.WebApi.Models
{
    public class PostLoginModel
    {
        [JsonRequired]
        public string Email { get; init; }

        [JsonRequired]
        public string Password { get; init; }

    }
}
