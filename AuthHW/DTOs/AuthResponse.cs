namespace AuthHW.DTOs;

public sealed class AuthenticationResult
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public int ExpiresInMinutes { get; init; }
}