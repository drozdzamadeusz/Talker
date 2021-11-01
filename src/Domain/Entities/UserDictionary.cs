using System.ComponentModel;
using talker.Domain.Common;

namespace talker.Domain.Entities
{
    public class UserDictionary : AuditableEntity
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public int ConversationId { get; set; }

        public bool UserRemoved { get; set; }
    }
}
