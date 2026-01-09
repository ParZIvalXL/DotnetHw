namespace AuthHW.Entities;

public class Message
{
    public long Id { get; set; }

    public int ChatId { get; set; }
    public Chat Chat { get; set; } = null!;

    public int SenderId { get; set; }
    public UserAccount Sender { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
}