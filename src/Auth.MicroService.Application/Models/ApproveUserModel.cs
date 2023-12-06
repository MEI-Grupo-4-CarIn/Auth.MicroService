using Auth.MicroService.Domain.Enums;

namespace Auth.MicroService.Application.Models
{
    public class ApproveUserModel
    {
        public int Id { get; init; }
        public Role? Role { get; init; }
    }
}
