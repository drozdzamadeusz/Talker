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
using talker.Application.Common.Helpers;
using talker.Application.Common.Interfaces;
using talker.Application.Users.Queries;
using talker.Domain.Entities;

namespace talker.Application.Conversations.Commands.MarkAsRead
{
    public class MarkAsReadCommand: IRequest
    {
        public int Id { get; set; }
    }

    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand>
    {

        private readonly IApplicationDbContext _context;
        private readonly ILogger<MarkAsReadCommandHandler> _logger;
        private readonly IMediator _mediator;

        public MarkAsReadCommandHandler(IApplicationDbContext context, ILogger<MarkAsReadCommandHandler> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetCurrentUserQuery());
            var userId = user.Id;

            var userIds = await _context.UsersConversations.Where(e => e.ConversationId == request.Id && !e.IsDeleted).ToListAsync(cancellationToken);

            if (HelperMethods.CheckIfUserBelongsToConversation(userIds, userId))
            {
                var conversationId = request.Id;

                var messages = await _context.Messages.Where(m =>
                                                                 m.CreatedBy != userId &&
                                                                 m.ConversationId == conversationId &&
                                                                 m.SeenBy.Where(u => u.UserId == userId).Count() < 1)
                                                       .ToListAsync(cancellationToken);


                if (messages == null || messages.Count() < 1)
                {
                    _logger.LogWarning("User already seen all messages");
                    return Unit.Value;
                }

                messages.ForEach(m =>
                {

                    m.SeenBy.Add(new UserMessage()
                    {
                        UserId = userId
                    });
                });

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }

            throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Message failure", "You are not a member of this conversation.")
                });
        }
    }
}
