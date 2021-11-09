namespace Core.Users.Models
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool Disabled { get; set; }
        
        public RoleResponse Role { get; set; }
    }
}