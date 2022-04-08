using System.Collections.Generic;
using talker.Domain.Common;
using talker.Domain.Enums;

namespace talker.Domain.Entities
{
    public class Message: AuditableEntity
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public string Content { get; set; }

        public MessageType Type { get; set; }

        public IList<UserMessage> SeenBy { get; set; } = new List<UserMessage>();
    }
}
