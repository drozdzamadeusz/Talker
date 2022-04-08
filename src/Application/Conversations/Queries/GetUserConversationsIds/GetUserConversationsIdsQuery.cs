using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using talker.Application.Common.Interfaces;

namespace talker.Application.Conversations.Queries.GetUserConversationsIds
{
    public class GetUserConversationsIdsQuery: IRequest<List<int>>
    {
        public string UserId { set; get; }
    }

    public class GetUserConversationsIdsQueryHandler : IRequestHandler<GetUserConversationsIdsQuery, List<int>>
    {

        private readonly IApplicationDbContext _context;

        public GetUserConversationsIdsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> Handle(GetUserConversationsIdsQuery request, CancellationToken cancellationToken)
        {

            var currentUserId = request.UserId;

            var conversationsIds = await _context.UsersConversations
                .Where(u => u.UserId == currentUserId && !u.IsDeleted)
                .OrderBy(k => k.Id)
                .ToListAsync(cancellationToken);

            var conversationsIdsList = new List<int>();

            conversationsIds.ForEach(c =>
            {
                conversationsIdsList.Add(c.ConversationId);
            });

            return conversationsIdsList;

        }
    }
}
