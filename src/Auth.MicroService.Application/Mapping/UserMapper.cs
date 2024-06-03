using System.Collections.Generic;
using Auth.MicroService.Application.Models;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Enums;
using Riok.Mapperly.Abstractions;

namespace Auth.MicroService.Application.Mapping
{
    [Mapper]
    public static partial class UserMapper
    {
        [MapperIgnoreSource(nameof(User.Password))]
        [MapProperty(nameof(User.RoleId), nameof(UserInfoResponseModel.Role))]
        public static partial UserInfoResponseModel UserToUserInfoResponseModel(User model);

        private static int UserIdToUserId(int? userId) => userId.Value;
        private static string RoleIdToRole(Role roleId) => roleId.ToString();
    }
}
