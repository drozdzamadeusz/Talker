using talker.Application.Common.Models;
using System.Threading.Tasks;
using talker.Application.Users.Queries;
using System.Collections.Generic;

namespace talker.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);

        Task<bool> IsInRoleAsync(string userId, string role);

        Task<bool> AuthorizeAsync(string userId, string policyName);

        Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

        Task<Result> DeleteUserAsync(string userId);

        Task<ApplicationUserDto> GetUserAsync(string userId);

        Task<List<ApplicationUserDto>> GetUsersAsync(List<string> userIds);

        Task<List<ApplicationUserDto>> FindUsersAsync(string userName, string firstName, string lastName);
    }
}
