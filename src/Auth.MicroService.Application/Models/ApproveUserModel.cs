using Auth.MicroService.Domain.Enums;

namespace Auth.MicroService.Application.Models
{
    public class ApproveUserModel
    {
        public int Id { get; set; }

        public Role? Role { get; set; }
    }
}
