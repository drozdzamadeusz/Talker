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
using talker.Domain.Entities;

namespace talker.Application.Conversations.Commands.AddUserToConversation
{
    public class AddUserToConversationCommand: IRequest
    {

        public int ConversationId { get; set; }
        public string UserId { get; set; }

    }

    public class AddUserToConversationCommandHandler : IRequestHandler<AddUserToConversationCommand>
    {

        private readonly IApplicationDbContext _context;
        private readonly ILogger<AddUserToConversationCommandHandler> _logger;
        private readonly IMediator _mediator;

        public AddUserToConversationCommandHandler(IApplicationDbContext context, ILogger<AddUserToConversationCommandHandler> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(AddUserToConversationCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetCurrentUserQuery());
            var currentUserId = user.Id;

            var currentUserEntity = await _context.UsersConversations.Where(i => i.ConversationId == request.ConversationId && i.UserId == currentUserId && !i.IsDeleted).SingleOrDefaultAsync();

            if (currentUserEntity == null)
            {
                throw new NotFoundException("User", request.ConversationId);
            }

            if (!(currentUserEntity.Role == Domain.Enums.ConversationRole.Creator ||
                currentUserEntity.Role == Domain.Enums.ConversationRole.Admin))
            {
                throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Conversation failure", "You don't have permissions to add users to this conversation.")
                });
            }


            var entity = await _context.UsersConversations.Where(i => i.ConversationId == request.ConversationId && i.UserId == request.UserId && !i.IsDeleted).SingleOrDefaultAsync();

            if (entity != null)
            {
                return Unit.Value;
            }

            var newEntity = new UserConversation
            {
                ConversationId = request.ConversationId,
                UserId = request.UserId,
                Role = Domain.Enums.ConversationRole.User
            };

            _context.UsersConversations.Add(newEntity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
