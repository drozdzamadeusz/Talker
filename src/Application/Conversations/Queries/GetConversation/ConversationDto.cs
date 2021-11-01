using System.Collections.Generic;
using talker.Application.Users.Queries;

namespace talker.Application.Conversations.Queries.GetConversation
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ApplicationUserDto> Users { get; set; }
    }
}
