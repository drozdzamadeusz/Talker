using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Common.Interfaces;
using talker.Application.Users.Queries;
using talker.Domain.Entities;

namespace talker.Application.Conversations.Commands.CreateConversation
{
    public class CreateConversationCommand : IRequest<int>
    {

        public string Name { get; set; }
        public List<UeserIdRequestDto> UsersIds { get; set; }

        public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, int>
        {

            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            private readonly IIdentityService _identityService;

            public CreateConversationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IIdentityService identityService)
            {
                _context = context;
                _currentUserService = currentUserService;
                _identityService = identityService;
            }

            public async Task<int> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
            {

                var userId = _currentUserService.UserId ?? string.Empty;
                ApplicationUserDto currentUser;

                if (!string.IsNullOrEmpty(userId))
                {
                    currentUser = await _identityService.GetUserAsync(userId);
                }
                else
                {
                    throw new ForbiddenAccessException();
                }

                var userIds = new List<UserDictionary>();

                request.UsersIds.ForEach(u =>
                {
                    userIds.Add(new UserDictionary
                    {
                        UserId = u.UserId
                    });
                });

                if (request.UsersIds.Where(u => u.UserId == userId).Count() <= 0)
                {
                    userIds.Add(new UserDictionary
                    {
                        UserId = userId
                    });
                }

                var entity = new Conversation
                {
                    Name = request.Name,
                    UsersIds = userIds
                };

                _context.Conversations.Add(entity);

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }

        }
    }
}
