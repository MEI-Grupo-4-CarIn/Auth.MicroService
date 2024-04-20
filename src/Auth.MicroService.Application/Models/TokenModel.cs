using System;

namespace Auth.MicroService.Application.Models;

public class TokenModel
{
    public string Token { get; init; }
    public long ExpiresIn { get; init; }
    public string RefreshToken { get; init; }     
}