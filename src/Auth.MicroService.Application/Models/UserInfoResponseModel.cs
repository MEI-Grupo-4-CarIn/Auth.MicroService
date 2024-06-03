using System;

namespace Auth.MicroService.Application.Models
{
    public record UserInfoResponseModel(
        int UserId,
        string FirstName,
        string LastName,
        string Email,
        DateTime BirthDate,
        string Role,
        bool Status,
        DateTime CreationDateUtc,
        DateTime? LastUpdateDateUtc);
}