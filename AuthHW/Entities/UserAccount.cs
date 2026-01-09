namespace AuthHW.Entities;


public class UserAccount
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;
    public string Tag { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигации
    public UserProfile Profile { get; set; } = null!;
    public ICollection<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
