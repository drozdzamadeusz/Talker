using talker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace talker.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Conversation> Conversations { get; set; }

        DbSet<Message> Messages { get; set; }

        DbSet<UserConversation> UsersConversations { get; set; }

        DbSet<UserMessage> UsersMessages { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<int> SaveChangesAsUserAsync(string userId, CancellationToken cancellationToken);
    }
}
