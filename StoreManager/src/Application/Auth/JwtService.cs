using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Auth.Interfaces;
using Core.Auth.Models;
using Core.Configurations.Extensions;
using Core.Errors;
using Core.Users.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Auth
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public BearerTokenResponse GenerateToken(AuthUserResponse user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var settings = _configuration.GetSettings();
            var jwtSecret = settings.AuthSettings.JwtSecret;
            var expireTime = Convert.ToInt32(settings.AuthSettings.JwtExpireTimesInMinuts);

            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new EnvironmentVariableNotFoundException(nameof(jwtSecret));
            }
            
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var claims = GenerateClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            var response = new BearerTokenResponse
            {
                Token = tokenHandler.WriteToken(token)
            };

            return response;
        }

        private IEnumerable<Claim> GenerateClaims(AuthUserResponse user)
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