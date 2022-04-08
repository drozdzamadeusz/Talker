using FluentValidation.Results;
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
using talker.Application.Common.Interfaces;
using talker.Application.Users.Queries;

namespace talker.Application.Conversations.Commands.RemoveUserAsAdmin
{
    public class RemoveUserAsAdminCommand : IRequest
    {
        public int ConversationId { get; set; }
        public string UserId { get; set; }

    }

    public class RemoveUserAsAdminCommandHandler : IRequestHandler<RemoveUserAsAdminCommand>
    {

        private readonly IApplicationDbContext _context;
        private readonly ILogger<RemoveUserAsAdminCommandHandler> _logger;
        private readonly IMediator _mediator;

        public RemoveUserAsAdminCommandHandler(IApplicationDbContext context, ILogger<RemoveUserAsAdminCommandHandler> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(RemoveUserAsAdminCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetCurrentUserQuery());
            var currentUserId = user.Id;

            var currentUserEntity = await _context.UsersConversations.Where(i => i.ConversationId == request.ConversationId && i.UserId == currentUserId && !i.IsDeleted).SingleOrDefaultAsync();

            if (currentUserEntity == null)
            {
                throw new NotFoundException("User", request.ConversationId);
            }

            if (!(currentUserEntity.Role == Domain.Enums.ConversationRole.Creator))
            {
                throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Conversation failure", "You don't have permissions to manage roles of this conversation.")
                });
            }

            var entity = await _context.UsersConversations.Where(i => i.ConversationId == request.ConversationId && i.UserId == request.UserId && !i.IsDeleted).SingleOrDefaultAsync();

            if (entity == null)
            {
                throw new NotFoundException("User", request.ConversationId);
            }

            if (entity.Role == Domain.Enums.ConversationRole.Creator)
            {
                throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Conversation failure", "You are not a member of this conversation.")
                });
            }

            entity.Role = Domain.Enums.ConversationRole.User;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
