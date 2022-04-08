using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Interfaces;

namespace talker.Application.Users.Queries
{
    public class FindUsersQuery: IRequest<List<ApplicationUserDto>>
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class FindUsersQueryHandler : IRequestHandler<FindUsersQuery, List<ApplicationUserDto>>
    {
        private readonly IIdentityService _identityService;

        public FindUsersQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<List<ApplicationUserDto>> Handle(FindUsersQuery request, CancellationToken cancellationToken)
        {
            return await _identityService.FindUsersAsync(request.UserName, request.FirstName, request.LastName);
        }
    }
}
