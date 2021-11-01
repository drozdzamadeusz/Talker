using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Interfaces;
using talker.Application.Users.Queries;

namespace talker.Application.Conversations.Queries.GetConversation
{
    public class GetConversationQuery : IRequest<ConversationDto>
    {

        public int Id { get; set; }

        public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, ConversationDto>
        {

            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly IIdentityService _identityService;

            public GetConversationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService)
            {
                _context = context;
                _mapper = mapper;
                _identityService = identityService;
            }

            public async Task<ConversationDto> Handle(GetConversationQuery request, CancellationToken cancellationToken)
            {

                var result = await _context.Conversations
                    .Where(i => i.Id == request.Id)
                    .Select(o => new ConversationDto
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Users = o.UsersIds
                                    .Select(k => _identityService.GetUserAsync(k.UserId).GetAwaiter().GetResult())
                                    .ToList()
                    })
                    .OrderBy(i => i.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                return result;
            }
        }

    }
}
