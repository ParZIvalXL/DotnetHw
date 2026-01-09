using AuthHW.DTOs.User;

namespace AuthHW.DTOs.Chat;

public class ChatListItemDto
{
    public int ChatId { get; set; }
    public string Type { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Avatar { get; set; }
    public bool Online { get; set; }

    public string? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }

    public PartnerDto? Partner { get; set; }
}
