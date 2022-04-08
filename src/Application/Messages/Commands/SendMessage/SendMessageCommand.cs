using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Common.Helpers;
using talker.Application.Common.Interfaces;
using talker.Application.Users.Queries;
using talker.Domain.Entities;

namespace talker.Application.Messages.Commands.SendMessage
{
    public class SendMessageCommand : IRequest<int>
    {
        public int ConversationId { get; set; }

        public string Content { get; set; }

        public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, int>
        {

            private readonly IApplicationDbContext _context;
            private readonly ILogger<SendMessageCommandHandler> _logger;
            private readonly IMediator _mediator;

            public SendMessageCommandHandler(IApplicationDbContext context, ILogger<SendMessageCommandHandler> logger, IMediator mediator)
            {
                _context = context;
                _logger = logger;
                _mediator = mediator;
            }

            public async Task<int> Handle(SendMessageCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = (await _mediator.Send(new GetCurrentUserQuery())).Id;

                var conversation = await _context.Conversations.Where(c => c.Id == request.ConversationId)
                                            .SingleOrDefaultAsync(cancellationToken);

                var userIds = await _context.UsersConversations.Where(e => e.ConversationId == conversation.Id && !e.IsDeleted).ToListAsync(cancellationToken);

                if (HelperMethods.CheckIfUserBelongsToConversation(userIds, currentUserId))
                {
                    var entity = new Message
                    {
                        Content = request.Content,
                        ConversationId = request.ConversationId
                    };

                    _context.Messages.Add(entity);
                    await _context.SaveChangesAsync(cancellationToken);

                    return entity.Id;
                }

                throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Message failure", "You are not a member of the conversation.")
                });
            }
        }
    }
}
