using Auth.MicroService.Application.Models;
using Auth.MicroService.WebApi.Models;
using Riok.Mapperly.Abstractions;

namespace Auth.MicroService.WebApi.Mapping
{
    [Mapper]
    public static partial class UserMapper
    {
        public static partial RegisterModel PostRegisterModelToRegisterModel(PostRegisterModel model);
    }
}
