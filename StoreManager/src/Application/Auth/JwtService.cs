using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Auth.Interfaces;
using Core.Configurations.Extensions;
using Core.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Auth
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var settings = ConfigurationExtension.GetSettings(_configuration);

            var key = Encoding.ASCII.GetBytes(settings.AuthSettings.JwtSecret);
            var expireTime = Convert.ToInt32(settings.AuthSettings.JwtExpireTimesInMinuts);

            var claims = GenerateClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private IEnumerable<Claim> GenerateClaims(User user)
        {
            var claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.PrimarySid, user.Id.ToString()),
            };
            
            AddRoleClaim(nameof(user.Role.IsAdmin), user.Role.IsAdmin, claims);

            return claims;
        }

        private void AddRoleClaim(string roleName, bool isRole, List<Claim> claims)
        {
            if (isRole)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }
        }
    }
}