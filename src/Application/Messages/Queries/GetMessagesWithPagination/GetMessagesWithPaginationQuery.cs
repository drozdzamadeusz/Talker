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
using talker.Application.Common.Mappings;
using talker.Application.Common.Models;
using talker.Application.Users.Queries;

namespace talker.Application.Messages.Queries.GetMessagesWithPagination
{
    public class GetMessagesWithPaginationQuery: IRequest<PaginatedList<MessageDto>>
    {
        public int ConversationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetMessagesWithPaginationQueryhandler : IRequestHandler<GetMessagesWithPaginationQuery, PaginatedList<MessageDto>>
    {

        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetMessagesWithPaginationQueryhandler(IApplicationDbContext context, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<PaginatedList<MessageDto>> Handle(GetMessagesWithPaginationQuery request, CancellationToken cancellationToken)
        {

            var currentUserId = (await _mediator.Send(new GetCurrentUserQuery())).Id;

            var conversation = await _context.Conversations.Where(c => c.Id == request.ConversationId)
                                        .SingleOrDefaultAsync(cancellationToken);

            var userIds = await _context.UsersConversations.Where(e => e.ConversationId == conversation.Id && !e.IsDeleted).ToListAsync(cancellationToken);

            if (!HelperMethods.CheckIfUserBelongsToConversation(userIds, currentUserId))
            {
                throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Message failure", "You are not a member of this conversation.")
                });
            }


            return await _context.Messages
                .Where(x => x.ConversationId == request.ConversationId)
                .OrderByDescending(x => x.Id)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }

}
