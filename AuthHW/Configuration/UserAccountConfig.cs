using AuthHW.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthHW.Configuration;

public sealed class UserAccountConfig : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Username)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(e => e.NormalizedUsername)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(e => e.PasswordHash)
            .IsRequired();
            
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");
            
        builder.HasIndex(e => e.NormalizedUsername)
            .IsUnique();
    }
}