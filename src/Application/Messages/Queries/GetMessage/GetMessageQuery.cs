using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Common.Helpers;
using talker.Application.Common.Interfaces;
using talker.Application.Users.Queries;

namespace talker.Application.Messages.Queries.GetMessage
{
    public class GetMessageQuery : IRequest<MessageDto>
    {
        public int Id { set; get; }
    }
    public class GetMessagesQueryHandler : IRequestHandler<GetMessageQuery, MessageDto>
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

        public async Task<MessageDto> Handle(GetMessageQuery request, CancellationToken cancellationToken)
        {

            var message = await _context.Messages
                     .Where(x => x.Id == request.Id && !x.IsDeleted)
                     .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken);

            if (message == null)
            {
                throw new NotFoundException("Message", request.Id);
            }

            var currentUserId = (await _mediator.Send(new GetCurrentUserQuery())).Id;

            var conversation = await _context.Conversations.Where(c => c.Id == message.ConversationId)
                                        .SingleOrDefaultAsync(cancellationToken);

            var userIds = await _context.UsersConversations.Where(e => e.ConversationId == conversation.Id && !e.IsDeleted).ToListAsync(cancellationToken);

            if (!HelperMethods.CheckIfUserBelongsToConversation(userIds, currentUserId))
            {
                throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Message failure", "You are not a member of the conversation.")
                });
            }

            return message;
        }
    }
}
