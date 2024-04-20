using System.Text.Json.Serialization;

namespace Auth.MicroService.WebApi.Models
{
    public class PostRefreshTokenModel
    {
        [JsonRequired]
        public string RefreshToken { get; init; }
    }
}
