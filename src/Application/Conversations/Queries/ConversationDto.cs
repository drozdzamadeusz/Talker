using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using talker.Application.Common.Interfaces;
using talker.Application.Common.Mappings;
using talker.Application.Messages;
using talker.Application.Users.Queries;
using talker.Domain.Entities;
using talker.Domain.Enums;

namespace talker.Application.Conversations.Queries.GetConversation
{
    public class ConversationDto : IDateTime, IMapFrom<Conversation>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ConversationColor Color { get; set; }
        public IList<UserConversationDto> Users { get; set; }
        public DateTimeOffset Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset Now => TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<Conversation, ConversationDto>()
                .ForMember(c => c.Users, opt =>
                    opt.MapFrom(src =>
                        src.UsersIds.Where(a => a.IsDeleted == false)));
        }
    }
}
