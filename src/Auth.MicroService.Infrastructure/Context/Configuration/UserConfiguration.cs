using Auth.MicroService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.MicroService.Infrastructure.Context.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder
                .HasKey(user => user.UserId);

            builder
                .Property(user => user.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(user => user.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(user => user.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(user => user.Password)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(user => user.BirthDate)
                .IsRequired();

            builder
                .Property(user => user.RoleId)
                .IsRequired();

            builder
                .Property(user => user.Status)
                .IsRequired();

            builder
                .Property(user => user.CreationDateUtc)
                .HasDefaultValueSql("GETDATE()");

            builder
                .Property(user => user.LastUpdateDateUtc)
                .IsRequired(false);
        }
    }
}
