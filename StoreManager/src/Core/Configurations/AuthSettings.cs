namespace Core.Configurations;

public class AuthSettings
{
    public string JwtSecret { get; set; }
    public long JwtExpireTimesInMinuts { get; set; }
    public string DefaulUserEmail { get; set; }
    public string DefaultUserPassword { get; set; }
}