using talker.Application.Common.Exceptions;
using talker.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Users.Queries;

namespace talker.Application.Users.Queries
{
    public class GetUserQuery: IRequest<ApplicationUserDto>
    {
        public string UserId { set; get; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ApplicationUserDto>
    {
        private readonly IIdentityService _identityService;

        public GetUserQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<ApplicationUserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId ?? string.Empty;
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
