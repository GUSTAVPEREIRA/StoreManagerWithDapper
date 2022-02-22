using Core.Auth.Models;
using Core.Users.Models;

namespace Core.Auth.Interfaces;

public interface IJwtService
{
    public BearerTokenResponse GenerateToken(AuthUserResponse user);
}