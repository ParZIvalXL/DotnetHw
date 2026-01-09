namespace AuthHW.Entities;

public enum ChatRole
{
    Member = 1,
    Admin = 2,
    Owner = 3
}

public class ChatParticipant
{
    public int ChatId { get; set; }
    public Chat Chat { get; set; } = null!;

    public int UserId { get; set; }
    public UserAccount User { get; set; } = null!;

    public ChatRole Role { get; set; } = ChatRole.Member;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastReadAt { get; set; }
}