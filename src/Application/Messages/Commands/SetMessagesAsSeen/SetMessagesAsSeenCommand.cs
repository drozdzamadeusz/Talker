using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Common.Helpers;
using talker.Application.Common.Interfaces;
using talker.Application.Users.Queries;
using talker.Domain.Entities;

namespace talker.Application.Messages.Commands.SetMessagesAsSeen
{
    public class SetMessagesAsSeenCommand : IRequest<int>
    {
        public int[] MessagesIds { get; set; }

        public int ConversationId { get; set; }

        public string UserId { get; set; }
    }

    public class SetMessagesAsSeenCommandHandler : IRequestHandler<SetMessagesAsSeenCommand, int>
    {

        private readonly IApplicationDbContext _context;
        private readonly ILogger<SetMessagesAsSeenCommandHandler> _logger;
        private readonly IMediator _mediator;

        public SetMessagesAsSeenCommandHandler(IApplicationDbContext context, ILogger<SetMessagesAsSeenCommandHandler> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<int> Handle(SetMessagesAsSeenCommand request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            var usersIds = await _context.UsersConversations.Where(e => e.ConversationId == request.ConversationId).ToListAsync(cancellationToken);

            if (userId != null && usersIds != null && HelperMethods.CheckIfUserBelongsToConversation(usersIds, userId))
            {

                var messagesIds = request.MessagesIds;
                var conversationId = request.ConversationId;

                var messages = await _context.Messages.Where(m =>
                                                                 m.CreatedBy != userId &&
                                                                 m.ConversationId == conversationId &&
                                                                 messagesIds.Contains(m.Id) &&
                                                                 m.SeenBy.Where(u => u.UserId == userId).Count() < 1)
                                                       .ToListAsync(cancellationToken);

                if (messages == null || messages.Count() < 1)
                {
                    _logger.LogWarning("User already seen all messages");
                    return -1;
                }

                messages.ForEach(m =>
                {

                    m.SeenBy.Add(new UserMessage()
                    {
                        UserId = userId
                    });
                });

                await _context.SaveChangesAsync(cancellationToken);

                return _context.Messages.Where(
                                        c =>
                                              c.ConversationId == conversationId &&
                                              c.SeenBy.Where(w => w.UserId == userId).Count() < 1 &&
                                              c.CreatedBy != userId
                                        ).Count();
            }

            throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Message failure", "You are not a member of the conversation.")
                }
            );
        }
    }
}
