namespace AuthHW.Settings;

public class TokenSettings
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int TokenValidityMinutes { get; init; } = 60;
}