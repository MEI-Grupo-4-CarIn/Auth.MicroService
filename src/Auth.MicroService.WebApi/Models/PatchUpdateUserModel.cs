using System;

namespace Auth.MicroService.WebApi.Models
{
    public class PatchUpdateUserModel
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string Email { get; init; }
    }
}
