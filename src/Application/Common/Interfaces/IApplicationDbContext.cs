using talker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace talker.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<TodoList> TodoLists { get; set; }

        DbSet<TodoItem> TodoItems { get; set; }

        DbSet<Conversation> Conversations { get; set; }

        DbSet<Message> Messages { get; set; }

        DbSet<UserDictionary> UserDictionary { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
