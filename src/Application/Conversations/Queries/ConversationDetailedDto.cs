using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using talker.Application.Common.Interfaces;
using talker.Application.Conversations.Queries.GetConversation;
using talker.Application.Messages;
using talker.Domain.Entities;

namespace talker.Application.Conversations.Queries
{
    public class ConversationDetailedDto: ConversationDto
    {
        public int UnseenMessages { get; set; }
        public MessageDto LastMessage { get; set; }

    }
}
