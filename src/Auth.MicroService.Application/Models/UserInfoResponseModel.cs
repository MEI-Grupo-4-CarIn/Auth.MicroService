namespace Auth.MicroService.Application.Models
{
    public record UserInfoResponseModel(
        int UserId,
        string UserFullName,
        string Email,
        string Role,
        bool Status,
        string CreationDate,
        string LastUpdateDate);
}