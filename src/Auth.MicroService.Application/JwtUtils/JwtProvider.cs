using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Auth.MicroService.Application.Models;

namespace Auth.MicroService.Application.JwtUtils
{
    public sealed class JwtProvider : IJwtProvider
    {
        private const int LongExpireSeconds = 600; // 10 minutes

        private readonly IConfiguration _configuration;

        public JwtProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenModel GenerateJwt(User user, bool needsRefreshToken = true)
        {
            var claims = new Claim[]
            {
               new("id", user.UserId.Value.ToString()),
               new("email", user.Email),
               new("firstName", user.FirstName),
               new("lastName", user.LastName),
               new ("role", ((int)user.RoleId).ToString())
            };

            return GenerateToken(claims, needsRefreshToken);
        }

        public TokenModel GeneratePasswordResetToken(User user)
        {
            var claims = new Claim[]
            {
                new ("Email", user.Email),
                new ("isPasswordReset", "true")
            };

            return GenerateToken(claims, false);
        }

        public bool ValidateToken(string token)
        {
            return ValidateTokenByClaim(token, "id") is not null;
        }

        public string ValidatePasswordResetToken(string token)
        {
            return ValidateTokenByClaim(
                token,
                "Email",
                isPasswordReset: true);
        }

        public int? GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Get the user ID from the token
            var userIdClaim = jwtToken.Claims.First(c => c.Type == "id");

            if (int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            return null;
        }

        public Role? GetUserRoleFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Get the user role from the token
            var userRoleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type.Equals("role", StringComparison.OrdinalIgnoreCase));
            if (userRoleClaim is not null && Enum.TryParse<Role>(userRoleClaim.Value, out var role))
            {
                return role;
            }

            return null;
        }

        private TokenModel GenerateToken(Claim[] claims, bool needsRefreshToken)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_configuration["JwtSettings:PrivateKey"]);

            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa.ExportParameters(true)),
                SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                claims,
                null,
                DateTime.UtcNow.AddSeconds(LongExpireSeconds),
                signingCredentials);

            return new TokenModel(
                new JwtSecurityTokenHandler().WriteToken(token),
                needsRefreshToken ? GenerateRefreshToken() : null,
                LongExpireSeconds);
        }
        
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        
        private string ValidateTokenByClaim(
            string token,
            string claimType,
            bool isPasswordReset = false)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_configuration["JwtSettings:PublicKey"]);

            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"],
                IssuerSigningKey = new RsaSecurityKey(rsa.ExportParameters(false))
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

                if (isPasswordReset)
                {
                    var passwordResetClaim = claimsPrincipal.FindFirstValue("isPasswordReset");

                    if (passwordResetClaim is null)
                    {
                        throw new ArgumentException("Invalid token.");
                    }
                }

                return claimsPrincipal.FindFirstValue(claimType);
            }
            catch
            {
                return null;
            }
        }
    }
}
