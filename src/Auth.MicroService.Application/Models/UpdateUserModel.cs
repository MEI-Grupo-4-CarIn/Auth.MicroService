using System;

namespace Auth.MicroService.Application.Models
{
    public class UpdateUserModel
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string Email { get; init; }
    }
}
