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
        
        builder.Property(e => e.Email)
            .HasMaxLength(320)
            .IsRequired(); 
        
        builder.Property(e => e.Tag)
            .HasMaxLength(64)
            .IsRequired();
            
        builder.Property(e => e.PasswordHash)
            .IsRequired();
            
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");
            
        builder.HasIndex(e => e.Tag)
            .IsUnique();
    }
}