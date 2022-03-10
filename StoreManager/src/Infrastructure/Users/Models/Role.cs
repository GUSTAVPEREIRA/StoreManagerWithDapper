using System.Collections.Generic;

namespace Infrastructure.Users.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
    public ICollection<User> Users { get; set; }
}