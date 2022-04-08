using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Helpers;
using talker.Application.Common.Interfaces;
using talker.Application.Conversations.Queries.GetConversation;
using talker.Application.Messages;
using talker.Application.Users.Queries;

namespace talker.Application.Updates.Queries.GetUpdates
{
    public class GetUpdatesQuery : IRequest<MessageUpdateDto>
    {
        public string UserId { get; set; }
        public int MessagesLimit { get; set; } = 50;
        public DateTime TimeOfLastEvnet { get; set; }
    }

    public class CheckUpdateQueryHandler : IRequestHandler<GetUpdatesQuery, MessageUpdateDto>
    {

        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<GetUserConversationsWithHandler> _logger;

        public CheckUpdateQueryHandler(IApplicationDbContext context, IMapper mapper, IMediator mediator, ILogger<GetUserConversationsWithHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<MessageUpdateDto> Handle(GetUpdatesQuery request, CancellationToken cancellationToken)
        {

            var currentUserId = request.UserId;

            var conversationsIds = await _context.UsersConversations
                .Where(u => u.UserId == currentUserId)
                .OrderBy(k => k.Id)
                .ToListAsync(cancellationToken);

            var conversationsIdsList = new List<int>();

            conversationsIds.ForEach(c =>
            {
                conversationsIdsList.Add(c.ConversationId);
            });

            var date = request.TimeOfLastEvnet;

            _logger.LogWarning($"User forced update check...");

            var newMessages = await _context.Messages
                                 .Where(x => conversationsIdsList.Contains(x.ConversationId) &&
                                            (
                                                x.Created.Date > date.Date
                                                ) || (
                                                    x.Created.Date == date.Date &&
                                                    x.Created.Hour > date.Hour
                                                ) || (
                                                    x.Created.Date == date.Date &&
                                                    x.Created.Hour == date.Hour &&
                                                    x.Created.Minute > date.Minute
                                                ) || (
                                                    x.Created.Date == date.Date &&
                                                    x.Created.Hour == date.Hour &&
                                                    x.Created.Minute == date.Minute &&
                                                    x.Created.Second > date.Second
                                                )
                                            )
                                 .OrderBy(x => x.Id)
                                 .Take(request.MessagesLimit)
                                 .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                                 .ToListAsync(cancellationToken);

            return new MessageUpdateDto
            {
                Messages = newMessages
            };
        }
    }
}
