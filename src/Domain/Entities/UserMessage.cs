using talker.Domain.Common;

namespace talker.Domain.Entities
{
    public class UserMessage: AuditableEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ConversationId { get; set; }

        public int MessageId { get; set; }
    }
}
