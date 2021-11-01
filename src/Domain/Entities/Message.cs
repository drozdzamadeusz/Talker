using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talker.Domain.Common;

namespace talker.Domain.Entities
{
    public class Message: AuditableEntity
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public string Content { get; set; }

        public IList<UserDictionary> SeenBy { get; set; } = new List<UserDictionary>();
    }
}
