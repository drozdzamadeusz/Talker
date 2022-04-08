using System.Collections.Generic;
using talker.Domain.Common;
using talker.Domain.Enums;

namespace talker.Domain.Entities
{
    public class Conversation: AuditableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ConversationColor Color { get; set; }

        public IList<UserConversation> UsersIds { get; set; } = new List<UserConversation>();

        public IList<Message> Messages { get; set; } = new List<Message>();
    }
}
