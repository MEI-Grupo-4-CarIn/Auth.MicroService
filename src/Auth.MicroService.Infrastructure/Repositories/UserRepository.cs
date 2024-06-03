using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Enums;
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

        public async Task<User> AddNewUser(User user, CancellationToken ct)
        {
            var databaseHasDefaultAdmin = await DatabaseHasDefaultAdminUser(ct);

            if (!databaseHasDefaultAdmin && user.Email.Equals(DefaultAdminEmail))
            {
                // Set the first user to be Admin and Status = true
                user = User.SetUserActivation(user, Role.Admin, status: true);
            }
            await _authDbContext.Set<User>().AddAsync(user, ct);
            await _authDbContext.SaveChangesAsync(ct);

            return user;
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

        public async Task<IEnumerable<User>> GetInactiveUsersList(int page, int perPage, CancellationToken ct)
        {
            int skip = (page - 1) * perPage;
            
            return await _authDbContext.Set<User>()
                .AsNoTracking()
                .Where(u => u.Status == false)
                .OrderByDescending(u => u.UserId)
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

        public async Task<IEnumerable<User>> GetUsersList(
            string search,
            Role? role,
            int page,
            int perPage,
            CancellationToken ct)
        {
            int skip = (page - 1) * perPage;
            var query = _authDbContext.Set<User>()
                .AsNoTracking();

            query = query.Where(u => u.Status == true);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(u => u.FirstName.ToLower().Contains(search)
                                         || u.LastName.ToLower().Contains(search)
                                         || u.Email.ToLower().Contains(search));
            }
            if (role is not null)
            {
                query = query.Where(u => u.RoleId == role);
            }

            query = query
                .OrderByDescending(u => u.UserId)
                .Skip(skip)
                .Take(perPage);

            return await query
                .ToListAsync(ct);
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
