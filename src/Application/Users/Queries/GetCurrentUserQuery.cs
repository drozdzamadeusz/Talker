using MediatR;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Common.Interfaces;

namespace talker.Application.Users.Queries
{
    public class GetCurrentUserQuery: IRequest<ApplicationUserDto>
    {
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ApplicationUserDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public GetCurrentUserQueryHandler(ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<ApplicationUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? string.Empty;
            ApplicationUserDto user;

            if (!string.IsNullOrEmpty(userId))
            {
                user = await _identityService.GetUserAsync(userId);
            }
            else
            {
                throw new NotFoundException();
            }

            return user;
        }
    }
}
