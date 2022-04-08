using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Common.Interfaces;
using talker.Application.Users.Queries;
using talker.Domain.Entities;
using talker.Domain.Enums;

namespace talker.Application.Messages.Commands.SendEventMessage
{
    public class SendEventMessageCommand : IRequest<SendEventResultDto>
    {
        public int ConversationId { get; set; }

        public MessageType Type { get; set; }

        public string? UserId { get; set; }

        public string? Value { get; set; }
    }

    public class SendMessageCommandHandler : IRequestHandler<SendEventMessageCommand, SendEventResultDto>
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

        public async Task<SendEventResultDto> Handle(SendEventMessageCommand request, CancellationToken cancellationToken)
        {

            var content = "";
            var affectedUser = "";

            if (request.UserId != null)
            {
                var user = await _mediator.Send(new GetUserQuery
                {
                    UserId = request.UserId,
                });

                affectedUser = (user != null) ? $"{user.FirstName} {user.LastName}" : "user";
            }

            switch (request.Type)
            {
                case MessageType.UserLeftConversation:
                    content = "left conversation";
                    break;
                case MessageType.RemovedUserFromConversation:
                    content = $"removed {affectedUser} from the conversation";
                    break;
                case MessageType.AddedUserToConversation:
                    content = $"added {affectedUser} to the conversation";
                    break;
                case MessageType.AdminGranted:
                    content = $"added {affectedUser} as a group admin";
                    break;
                case MessageType.AdminRevoked:
                    content = $"removed {affectedUser} as a group admin";
                    break;
                case MessageType.ConversationNameChanged:
                    content = $"changed conversation name" + ((request.Value != null) ? $" to {request.Value}" : "");
                    break;
                case MessageType.ConversationColorChanged:
                    content = $"changed conversation color" + ((request.Value != null) ? $" to {request.Value}" : "");
                    break;
                default:
                    content = "updated conversation";
                    break;
            }


            var entity = new Message
            {
                Content = content,
                Type = request.Type,
                ConversationId = request.ConversationId
            };

            _context.Messages.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new SendEventResultDto()
            {
                Id = entity.Id,
                Content = content,
                Type = request.Type
            };
        }
    }
}