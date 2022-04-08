using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Common.Helpers;
using talker.Application.Common.Interfaces;
using talker.Application.Messages.Queries.GetMessagesWithPagination;
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
            private readonly IMediator _mediator;

            public GetConversationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IMediator mediator)
            {
                _context = context;
                _mapper = mapper;
                _identityService = identityService;
                _mediator = mediator;
            }

            public async Task<ConversationDto> Handle(GetConversationQuery request, CancellationToken cancellationToken)
            {

                var user = await _mediator.Send(new GetCurrentUserQuery());

                var userIds = await _context.UsersConversations.Where(e => e.ConversationId == request.Id && !e.IsDeleted).ToListAsync(cancellationToken);

                if (HelperMethods.CheckIfUserBelongsToConversation(userIds, user.Id))
                {

                    var result = await _context.Conversations
                        .Where(i => i.Id == request.Id)
                        .OrderBy(i => i.Id)
                        .ProjectTo<ConversationDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);

                    return result;
                }

                throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Message failure", "You are not a member of the conversation.")
                });
            }
        }

    }
}
