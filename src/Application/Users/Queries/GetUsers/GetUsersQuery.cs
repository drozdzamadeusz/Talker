using talker.Application.Common.Exceptions;
using talker.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Users.Queries;
using System.Collections.Generic;

namespace talker.Application.Users.Queries.GetUsers
{
    public class GetUsersQuery: IRequest<List<ApplicationUserDto>>
    {
        public List<string> UserIds { set; get; } = new List<string>();
    }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<ApplicationUserDto>>
    {
        private readonly IIdentityService _identityService;

        public GetUsersQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<List<ApplicationUserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {

            return await _identityService.GetUsersAsync(request.UserIds);
        }
    }
}
