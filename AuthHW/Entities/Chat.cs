namespace AuthHW.Entities;

public enum ChatType
{
    Private = 1,
    Group = 2
}

public class Chat
{
    public int Id { get; set; }

    public ChatType Type { get; set; }

    public string? Title { get; set; } // только для групп
    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}