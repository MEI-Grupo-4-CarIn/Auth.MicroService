using System;

namespace Auth.MicroService.Application.Models
{
    public class RegisterModel
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public DateTime BirthDate { get; init; }
    }
}
