using Auth.MicroService.Application.JwtUtils;
using Auth.MicroService.Application.Services.Interfaces;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Application.Services
{
    /// <summary>
    /// The users service.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;

        public UsersService(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserInfo>> GetAllUsersForApproval(CancellationToken ct)
        {
            return await _userRepository.GetAllInactiveUsers(ct);
        }

        public async Task<bool> DeleteUser(int id, CancellationToken ct)
        {
            var user = await _userRepository.GetUserById(id, ct);

            if(user is null)
            {
                return false;
            }

            var deactivatedUser = User.SetUserActivation(user, null, false);

            await _userRepository.UpdateUser(deactivatedUser, ct);
            return true;
        }
    }
}
