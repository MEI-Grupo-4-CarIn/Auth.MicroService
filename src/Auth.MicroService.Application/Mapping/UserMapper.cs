using System.Collections.Generic;
using Auth.MicroService.Application.Models;
using Auth.MicroService.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Auth.MicroService.Application.Mapping
{
    [Mapper]
    public static partial class UserMapper
    {
        public static partial IEnumerable<UserInfoResponseModel> UserInfoToUserInfoResponseModel(IEnumerable<UserInfo> model);
    }
}
