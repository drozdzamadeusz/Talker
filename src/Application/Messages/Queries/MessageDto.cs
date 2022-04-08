using AutoMapper;
using System;
using System.Collections.Generic;
using talker.Application.Common.Interfaces;
using talker.Application.Common.Mappings;
using talker.Domain.Entities;
using talker.Domain.Enums;

namespace talker.Application.Messages
{
    public class MessageDto : IMapFrom<Message>
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public string Content { get; set; }

        public DateTimeOffset Created { get; set; }

        public string CreatedBy { get; set; }

        public MessageType Type { get; set; }

        public IList<SeenByDto> SeenBy { get; set; } = new List<SeenByDto>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Message, MessageDto>();
        }
    }
}
