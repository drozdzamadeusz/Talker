using talker.Domain.Common;
using System.Threading.Tasks;

namespace talker.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
