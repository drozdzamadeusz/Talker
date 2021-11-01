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
            private readonly ICurrentUserService _currentUserService;
            private readonly IIdentityService _identityService;
            private readonly ILogger<SendMessageCommandHandler> _logger;

            public SendMessageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IIdentityService identityService, ILogger<SendMessageCommandHandler> logger)
            {
                _context = context;
                _currentUserService = currentUserService;
                _identityService = identityService;
                _logger = logger;
            }

            public async Task<int> Handle(SendMessageCommand request, CancellationToken cancellationToken)
            {
                var userId = _currentUserService.UserId ?? string.Empty;
                ApplicationUserDto currentUser;

                if (!string.IsNullOrEmpty(userId))
                {
                    currentUser = await _identityService.GetUserAsync(userId);
                }
                else
                {
                    throw new ForbiddenAccessException();
                }


                var conversation = await _context.Conversations.Where(c => c.Id == request.ConversationId)
                                            .SingleOrDefaultAsync(cancellationToken);

                conversation.UsersIds = await _context.UserDictionary.Where(e => e.ConversationId == conversation.Id).ToListAsync(cancellationToken);

                _logger.LogWarning(JsonSerializer.Serialize(conversation));
       

                if (HelperMethods.CheckIfUserBelongsToConversation(conversation, userId))
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
                    new ValidationFailure("Message sending failure", "You are not a member of this conversation.")
                });
            }
        }
    }
}
