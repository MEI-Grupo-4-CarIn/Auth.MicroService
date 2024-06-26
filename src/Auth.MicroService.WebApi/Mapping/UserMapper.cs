﻿using Auth.MicroService.Application.Models;
using Auth.MicroService.WebApi.Models;
using Riok.Mapperly.Abstractions;

namespace Auth.MicroService.WebApi.Mapping
{
    [Mapper]
    public static partial class UserMapper
    {
        public static partial RegisterModel PostRegisterModelToRegisterModel(PostRegisterModel model);
        public static partial LoginModel PostLoginModelToLoginModel(PostLoginModel model);
        public static partial LogoutModel PostLogoutModelToLogoutModel(PostLogoutModel model);
        public static partial UpdateUserModel PatchUpdateUserModelToUpdateUserModel(PatchUpdateUserModel model);
        public static partial ResetPasswordModel PostResetPasswordModelToResetPasswordModel(PostResetPasswordModel model);
        public static partial ChangePasswordModel PostChangePasswordModelToChangePasswordModel(PostChangePasswordModel model);
    }
}
