namespace AuthHW.Entities;

public class AuthAttempt
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool WasSuccessful { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    public DateTime AttemptTime { get; set; } = DateTime.UtcNow;
}