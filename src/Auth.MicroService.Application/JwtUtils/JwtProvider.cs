using Auth.MicroService.Domain.Entities;
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

            string tokenValue = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return tokenValue;
        }

        public bool ValidateToken(string token)
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
                tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return false;
            }

            return true;
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
    }
}
