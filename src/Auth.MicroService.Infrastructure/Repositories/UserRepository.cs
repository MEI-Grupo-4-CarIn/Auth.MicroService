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
        private const string DefaultAdminEmail = "admin@email.com";
        
        private readonly AuthDbContext _authDbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task AddNewUser(User user, CancellationToken ct)
        {
            var databaseHasDefaultAdmin = await this.DatabaseHasDefaultAdminUser(ct);

            if (!databaseHasDefaultAdmin && user.Email.Equals(DefaultAdminEmail))
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

        public async Task<IEnumerable<UserInfo>> GetInactiveUsersList(int page, int perPage, CancellationToken ct)
        {
            int skip = (page - 1) * perPage;
            
            return await _authDbContext.Set<User>()
                .AsNoTracking()
                .Where(u => u.Status == false)
                .Select(u => new UserInfo
                {
                    UserId = u.UserId.Value,
                    UserFullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Role = u.RoleId.GetDescription(),
                    Status = u.Status,
                    CreationDate = u.CreationDateUtc.ToString("g"),
                    LastUpdateDate = u.LastUpdateDateUtc.HasValue ? u.LastUpdateDateUtc.Value.ToString("g") : "No updates",
                })
                .Skip(skip)
                .Take(perPage)
                .ToListAsync(ct);
        }

        public async Task<string> UpdateUser(User user, CancellationToken ct)
        {
            var existingUser = await _authDbContext.Set<User>()
                .SingleOrDefaultAsync(u => u.UserId == user.UserId, cancellationToken: ct);

            if (existingUser is null)
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

        public async Task<IEnumerable<UserInfo>> GetUsersList(string search, int page, int perPage, CancellationToken ct)
        {
            int skip = (page - 1) * perPage;

            var query = _authDbContext.Set<User>()
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.FirstName.ToLower().Contains(search.ToLower())
                                         || u.LastName.ToLower().Contains(search.ToLower())
                                         || u.Email.ToLower().Contains(search.ToLower()));
            }

            query = query.Skip(skip).Take(perPage);

            return await query
                .Select(u => new UserInfo
                {
                    UserId = u.UserId.Value,
                    UserFullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Role = u.RoleId.GetDescription(),
                    Status = u.Status,
                    CreationDate = u.CreationDateUtc.ToString("g"),
                    LastUpdateDate = u.LastUpdateDateUtc.HasValue ? u.LastUpdateDateUtc.Value.ToString("g") : "No updates",
                })
                .ToListAsync(ct);
        }

        public async Task<UserInfo> GetUserInfoById(int id, CancellationToken ct)
        {
            var user = await _authDbContext.Set<User>()
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserId == id, ct);
            
            if (user is null)
            {
                return null;
            }

            return new UserInfo
            {
                UserId = user.UserId.Value,
                UserFullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.RoleId.GetDescription(),
                Status = user.Status,
                CreationDate = user.CreationDateUtc.ToString("g"),
                LastUpdateDate = user.LastUpdateDateUtc.HasValue ? user.LastUpdateDateUtc.Value.ToString("g") : "No updates",
            };
        }

        private async Task<bool> DatabaseHasDefaultAdminUser(CancellationToken ct)
        {
            return await _authDbContext.Set<User>()
                .AnyAsync(u => 
                    u.Email.Equals(DefaultAdminEmail)
                    && u.RoleId == Role.Admin
                    && u.Status == true,
                    ct);
        }
    }
}
