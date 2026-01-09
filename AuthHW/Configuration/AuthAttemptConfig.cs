using AuthHW.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthHW.Configuration;

public sealed class AuthAttemptConfig : IEntityTypeConfiguration<AuthAttempt>
{
    public void Configure(EntityTypeBuilder<AuthAttempt> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Username)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(e => e.ClientIp)
            .HasMaxLength(100);
            
        builder.Property(e => e.UserAgent)
            .HasMaxLength(512);
            
        builder.Property(e => e.AttemptTime)
            .HasDefaultValueSql("now()");
    }
}