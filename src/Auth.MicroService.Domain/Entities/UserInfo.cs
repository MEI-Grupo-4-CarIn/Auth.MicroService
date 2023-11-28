using Auth.MicroService.Domain.Enums;

namespace Auth.MicroService.Domain.Entities
{
    public class UserInfo
    {
        public int UserId { get; init; }
        public string UserFullName { get; init; }
        public string Email { get; init; }
        public Role Role { get; init; }
    }
}
