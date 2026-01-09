namespace AuthHW.DTOs.User;

public class PartnerDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? Avatar { get; set; }
    public bool Online { get; set; }
    public DateTime? LastSeenAt { get; set; }
}