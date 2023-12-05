using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Enums;
using Auth.MicroService.Domain.Extensions;
using Auth.MicroService.Domain.Repositories;
using Auth.MicroService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _authDbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task AddNewUser(User user, CancellationToken ct)
        {
            var databaseHasActiveUsers = await this.DatabaseHasActiveUsers(ct);

            if (!databaseHasActiveUsers)
            {
                // Set the first user to be Admin and Status = true
                user = User.SetUserActivation(user, Role.Admin, status: true);
            }
            await _authDbContext.Set<User>().AddAsync(user, ct);
            await _authDbContext.SaveChangesAsync(ct);
        }

        public async Task<User> GetUserByEmail(string email, CancellationToken ct)
        {
            return await _authDbContext.Set<User>()
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<User> GetUserById(int id, CancellationToken ct)
        {
            return await _authDbContext.Set<User>()
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserId == id, ct);
        }

        public async Task<IEnumerable<UserInfo>> GetAllInactiveUsers(CancellationToken ct)
        {
            return await _authDbContext.Set<User>()
                .AsNoTracking()
                .Where(u => u.Status == false)
                .Select(u => new UserInfo
                {
                    UserId = u.UserId.Value,
                    UserFullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Role = u.RoleId.GetDescription(),
                    Status = u.Status
                })
                .ToListAsync(ct);
        }

        public async Task<string> UpdateUser(User user, CancellationToken ct)
        {
            var existingUser = await _authDbContext.Set<User>()
                .SingleOrDefaultAsync(u => u.UserId == user.UserId);              
               
            if(existingUser is null)
            {
                return null;
            }

            existingUser.Update(
                user.FirstName,
                user.LastName,
                user.Email,
                user.Password,
                user.RoleId,
                user.Status);

            await _authDbContext.SaveChangesAsync(ct);
            
            return existingUser.Email;
        }

        public async Task<IEnumerable<UserInfo>> GetAllUsers(CancellationToken ct)
        {           
            return await _authDbContext.Set<User>()
                .AsNoTracking()                
                .Select(u => new UserInfo
                {
                    UserId = u.UserId.Value,
                    UserFullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Role = u.RoleId.GetDescription(),
                    Status = u.Status
                })
                .ToListAsync(ct);
        }

        private async Task<bool> DatabaseHasActiveUsers (CancellationToken ct)
        {
            return await _authDbContext.Set<User>()
                .AnyAsync(u => u.Status == true, ct);
        }
    }
}
