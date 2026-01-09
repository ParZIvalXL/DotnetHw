using System.ComponentModel.DataAnnotations;

namespace AuthHW.Entities;

public class UserProfile
{
    [Key]
    public int UserId { get; set; }

    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
    // Навигация
    public UserAccount User { get; set; } = null!;
}