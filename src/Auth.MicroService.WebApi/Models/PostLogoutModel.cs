using System.Text.Json.Serialization;

namespace Auth.MicroService.WebApi.Models
{
    public class PostLogoutModel
    {
        [JsonRequired]
        public string RefreshToken { get; init; }
    }
}
