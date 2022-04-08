using talker.Domain.Common;
using talker.Domain.Enums;

namespace talker.Domain.Entities
{
    public class UserConversation : AuditableEntity
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public int ConversationId { get; set; }

        public ConversationRole Role { get; set; }
    }
}
