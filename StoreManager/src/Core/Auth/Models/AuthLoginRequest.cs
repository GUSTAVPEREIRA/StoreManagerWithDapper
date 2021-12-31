namespace Core.Auth.Models
{
    public class AuthLoginRequest
    {
        /// <example>something@host.com</example>
        public string Email { get; set; }
        
        /// <example>123456</example>
        public string Password { get; set; }
    }
}