using talker.Application.Common.Interfaces;
using talker.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using talker.Application.Users.Queries;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;

namespace talker.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public IdentityService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, IAuthorizationService authorizationService, IMapper mapper)
        {
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationService = authorizationService;
            _mapper = mapper;
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return user.UserName;
        }

        public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
            };

            var result = await _userManager.CreateAsync(user, password);

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            var result = await _authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded;
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return await DeleteUserAsync(user);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }

        public async Task<ApplicationUserDto> GetUserAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            var mapper = new Mapper(new MapperConfiguration(c => c.CreateMap<ApplicationUser, ApplicationUserDto>()));

            return mapper.Map<ApplicationUserDto>(user);
        }

        public async Task<List<ApplicationUserDto>> GetUsersAsync(List<string> userIds)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, ApplicationUserDto>();
            });

            return await _userManager.Users.Where(u => userIds.Contains(u.Id)).ProjectTo<ApplicationUserDto>(configuration).ToListAsync();
        }

        public async Task<List<ApplicationUserDto>> FindUsersAsync(string userName, string firstName, string lastName)
        {


            List<ApplicationUserDto> users;

            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(lastName)){
                users = await _userManager.Users.Where(u => u.UserName.Contains(firstName) ||
                                                            u.FirstName.Contains(firstName) ||
                                                            u.LastName.Contains(firstName))
                                   .ProjectTo<ApplicationUserDto>(new MapperConfiguration(c => c.CreateMap<ApplicationUser, ApplicationUserDto>()))
                                   .ToListAsync();
            }
            else {
                users = await _userManager.Users.Where(u => (string.IsNullOrEmpty(userName) || u.UserName.Contains(userName)) &&
                                                             (string.IsNullOrEmpty(firstName) || u.FirstName.Contains(firstName)) &&
                                                             (string.IsNullOrEmpty(lastName) || u.LastName.Contains(lastName)))
                                   .ProjectTo<ApplicationUserDto>(new MapperConfiguration(c => c.CreateMap<ApplicationUser, ApplicationUserDto>()))
                                   .ToListAsync();
            }
            return users;
        }
    }
}
