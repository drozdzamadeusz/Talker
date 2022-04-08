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
using talker.Domain.Enums;

namespace talker.Application.Conversations.Commands.UpdateConversation
{
    public class UpdateConversationCommand : IRequest<int>
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public ConversationColor? Color { get; set; }
    }

    public class UpdateConversationCommandHandler : IRequestHandler<UpdateConversationCommand, int>
    {

        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateConversationCommandHandler> _logger;
        private readonly IMediator _mediator;

        public UpdateConversationCommandHandler(IApplicationDbContext context, ILogger<UpdateConversationCommandHandler> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<int> Handle(UpdateConversationCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetCurrentUserQuery());
            var currentUserId = user.Id;

            var currentUserEntity = await _context.UsersConversations.Where(i => i.ConversationId == request.Id && i.UserId == currentUserId && !i.IsDeleted).SingleOrDefaultAsync();

            if (currentUserEntity == null)
            {
                throw new NotFoundException("User", request.Id);
            }

            if (!(currentUserEntity.Role == Domain.Enums.ConversationRole.Creator ||
                currentUserEntity.Role == Domain.Enums.ConversationRole.Admin))
            {
                throw new InvalidOperation(new List<ValidationFailure> {
                    new ValidationFailure("Conversation failure", "You don't have permissions to edit this conversation.")
                });
            }


            var entity = await _context.Conversations.Where(i => i.Id == request.Id && !i.IsDeleted).SingleOrDefaultAsync();

            if (entity == null)
            {
                throw new NotFoundException("Conversation", request.Id);
            }

            var result = 0;

            if(request.Name != null && request.Name != entity.Name)
            {
                result = 1;
                entity.Name = request.Name;
            }

            if (request.Color != null &&  request.Color != entity.Color)
            {
                result = 2;
                entity.Color = (ConversationColor) request.Color;
            }


            if (request.Name != null && request.Name != entity.Name && request.Color != entity.Color)
            {
                result = 3;
                entity.Name = request.Name;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
