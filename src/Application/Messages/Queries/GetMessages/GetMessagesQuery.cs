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
using talker.Application.Users.Queries;

namespace talker.Application.Messages.Queries.GetMessages
{
    public class GetMessagesQuery: IRequest<MessagesVm>
    {
        public int ConversationId { set; get; }

        public int LastMessageId { set; get; } = 0;

        public int MessagesLimit { get; set; } = 50;

        public bool FetchOldMessages { get; set; } = false;
    }

    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, MessagesVm>
    {

        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetMessagesQueryHandler(IApplicationDbContext context, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<MessagesVm> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = (await _mediator.Send(new GetCurrentUserQuery())).Id;

            var conversation = await _context.Conversations.Where(c => c.Id == request.ConversationId)
                                        .SingleOrDefaultAsync(cancellationToken);

            var userIds = await _context.UsersConversations.Where(e => e.ConversationId == conversation.Id && !e.IsDeleted).ToListAsync(cancellationToken);

            if (!HelperMethods.CheckIfUserBelongsToConversation(userIds, currentUserId))
            {
                throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Messages failure", "You are not a member of the conversation.")
                });
            }

            var messages = await _context.Messages
                                 .Where(x => x.ConversationId == request.ConversationId && !x.IsDeleted &&
                                       (request.FetchOldMessages && (request.LastMessageId == 0 || x.Id < request.LastMessageId) ||
                                       !request.FetchOldMessages && (request.LastMessageId == 0 || x.Id > request.LastMessageId)))
                                 .OrderByDescending(x => x.Id)
                                 .Take(request.MessagesLimit)
                                 .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                                 .ToListAsync(cancellationToken);

            return new MessagesVm
            {
                Messages = messages
            };
        }
    }
}
