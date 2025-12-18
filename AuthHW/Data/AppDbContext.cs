using AuthHW.Configuration;
using AuthHW.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthHW.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<AuthAttempt> AuthAttempts => Set<AuthAttempt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserAccountConfig());
        modelBuilder.ApplyConfiguration(new AuthAttemptConfig());
    }
}