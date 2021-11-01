using System.Collections.Generic;
using talker.Domain.Common;

namespace talker.Domain.Entities
{
    public class Conversation: AuditableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<UserDictionary> UsersIds { get; set; } = new List<UserDictionary>();
    }
}
