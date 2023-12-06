using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Auth.MicroService.Application.JwtUtils
{
    public sealed class JwtProvider : IJwtProvider
    {
        private const string EmailClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        private readonly IConfiguration _configuration;

        public JwtProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwt(User user)
        {
            var claims = new Claim[]
            {
               new("id", user.UserId.Value.ToString()),
               new(JwtRegisteredClaimNames.Email, user.Email),
               new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            return GenerateToken(claims);
        }

        public string GeneratePasswordResetToken(User user)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("isPasswordReset", "true")
            };

            return GenerateToken(claims);
        }

        public bool ValidateToken(string token)
        {
            return ValidateTokenByClaim(token, "id") is not null;
        }

        public string ValidatePasswordResetToken(string token)
        {
            return ValidateTokenByClaim(
                token,
                EmailClaimType,
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
            else
            {
                return null;
            }
        }

        public Role? GetUserRoleFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Get the user role from the token
            var userRoleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (userRoleClaim is not null && Enum.TryParse<Role>(userRoleClaim.Value, out var role))
            {
                return role;
            }
            else
            {
                return null;
            }
        }

        private string GenerateToken(Claim[] claims)
        {
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                claims,
                null,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string ValidateTokenByClaim(
            string token,
            string claimType,
            bool isPasswordReset = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]))
            };

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
