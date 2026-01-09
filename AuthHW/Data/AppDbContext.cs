using AuthHW.Configuration;
using AuthHW.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthHW.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatParticipant> ChatParticipants => Set<ChatParticipant>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<AuthAttempt> AuthAttempts => Set<AuthAttempt>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<UserAccount>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId);

        model.Entity<ChatParticipant>()
            .HasKey(cp => new { cp.ChatId, cp.UserId });

        model.Entity<UserAccount>()
            .HasIndex(u => u.Tag)
            .IsUnique();

        base.OnModelCreating(model);
    }
}