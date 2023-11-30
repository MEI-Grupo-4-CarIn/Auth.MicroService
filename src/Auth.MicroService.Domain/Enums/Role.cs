using System.ComponentModel;

namespace Auth.MicroService.Domain.Enums
{
    public enum Role
    {
        [Description("Administrator")]
        Admin = 1,

        [Description("Manager")]
        Manager = 2,

        [Description("Driver")]
        Driver = 3,
    }
}
