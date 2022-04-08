using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

namespace talker.Application.Conversations.Commands.LeaveConversation
{
    public class LeaveConversationCommand: IRequest
    {
        public int Id { get; set; }
    }

    public class LeaveConversationCommandHandler : IRequestHandler<LeaveConversationCommand>
    {

        private readonly IApplicationDbContext _context;
        private readonly ILogger<LeaveConversationCommandHandler> _logger;
        private readonly IMediator _mediator;

        public LeaveConversationCommandHandler(IApplicationDbContext context, ILogger<LeaveConversationCommandHandler> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(LeaveConversationCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetCurrentUserQuery());
            var userId = user.Id;


            var entity = await _context.UsersConversations.Where(i => i.ConversationId == request.Id && i.UserId == userId && !i.IsDeleted).SingleOrDefaultAsync();

            if (entity == null)
            {
                throw new NotFoundException("User", request.Id);
            }

            _context.UsersConversations.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
