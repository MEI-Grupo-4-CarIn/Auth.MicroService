using System.Text.Json.Serialization;

namespace Auth.MicroService.WebApi.Models
{
    public class PostValidateTokenModel
    {
        [JsonRequired]
        public string Token { get; init; }
    }
}
