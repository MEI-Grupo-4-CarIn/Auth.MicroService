using System;
using System.Text.Json.Serialization;

namespace Auth.MicroService.WebApi.Models
{
    public class PostRegisterModel
    {
        [JsonRequired]
        public string FirstName { get; init; }

        [JsonRequired]
        public string LastName { get; init; }

        [JsonRequired]
        public string Email { get; init; }

        [JsonRequired]
        public string Password { get; init; }

        [JsonRequired]
        public DateTime BirthDate { get; init; }
    }
}
