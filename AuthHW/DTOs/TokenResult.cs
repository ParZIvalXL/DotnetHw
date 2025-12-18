namespace AuthHW.DTOs;

public sealed class TokenResult
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string? Token { get; init; }
    public int? ExpirationMinutes { get; init; }
}