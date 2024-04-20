using Auth.MicroService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.MicroService.Infrastructure.Context.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.RefreshTokenId);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(rt => rt.ExpiresIn)
                .IsRequired();

            builder.Property(rt => rt.CreatedDateUtc)
                .IsRequired();

            builder.Property(rt => rt.IsRevoked)
                .IsRequired();

            builder.Property(rt => rt.RevokedDateUtc)
                .IsRequired(false);

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId);
        }
    }
}
