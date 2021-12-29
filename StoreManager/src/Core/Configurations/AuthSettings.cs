namespace Core.Configurations;

public class AuthSettings
{
    public string JwtSecret { get; set; }
    public long JwtExpireTimesInMinuts { get; set; }
}