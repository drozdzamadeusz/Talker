using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Common.Interfaces;
using talker.Application.Messages;
using talker.Application.Users.Queries;
using talker.Domain.Entities;

namespace talker.Application.Conversations.Queries.GetConversation
{
    public class GetUserConversationsQuery : IRequest<List<ConversationDetailedDto>>
    {
    }

    public class GetUserConversationsWithHandler : IRequestHandler<GetUserConversationsQuery, List<ConversationDetailedDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUserConversationsWithHandler> _logger;

        public GetUserConversationsWithHandler(IApplicationDbContext context, IMapper mapper, IMediator mediator, IIdentityService identityService, ILogger<GetUserConversationsWithHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<List<ConversationDetailedDto>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
            var currentUserId = currentUser.Id;

            var conversationsIds = await _context.UsersConversations
                .Where(u => u.UserId == currentUserId && !u.IsDeleted)
                .OrderBy(k => k.Id)
                .ToListAsync(cancellationToken);

            var conversationsIdsList = new List<int>();

            conversationsIds.ForEach(c =>
            {
                conversationsIdsList.Add(c.ConversationId);
            });

            var configuration = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserConversation, UserConversationDto>();
                cfg.CreateMap<Message, MessageDto>();
                cfg.CreateMap<UserMessage, SeenByDto>();

                cfg.CreateMap<Conversation, ConversationDetailedDto>()
                   .ForMember(c => c.Users, opt =>
                        opt.MapFrom(src =>
                                src.UsersIds.Where(a => !a.IsDeleted)
                            )
                        )
                    .ForMember(c => c.UnseenMessages, opt =>
                        opt.MapFrom(src =>
                                src.Messages.Where(m => 
                                    m.SeenBy.Where(w => w.UserId == currentUserId).Count() < 1 && 
                                    m.CreatedBy != currentUserId && 
                                    !m.IsDeleted
                                ).Count()
                            )
                        )
                   .ForMember(c => c.LastMessage, opt =>
                        opt.MapFrom(src =>
                                src.Messages.OrderByDescending(m => m.Id).Take(1).SingleOrDefault()
                            )
                        );
                }
            );


            return await _context.Conversations
                .Where(i => conversationsIdsList.Contains(i.Id))
                .OrderBy(i => i.Id)
                .ProjectTo<ConversationDetailedDto>(configuration)
                .ToListAsync(cancellationToken);
        }
    }
}
