namespace Core.Users.Models
{
    public class UserUpdatedRequest : UserRequest
    {
        public int Id { get; set; }
        public bool Disabled { get; set; }
    }
}