using Core.Users;

namespace Core.Auth.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}