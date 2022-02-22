namespace Core.Users;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public bool Disabled { get; set; }
    public Role Role { get; set; }
}