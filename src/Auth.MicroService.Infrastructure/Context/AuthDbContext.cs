using Auth.MicroService.Infrastructure.Context.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Auth.MicroService.Infrastructure.Context
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
