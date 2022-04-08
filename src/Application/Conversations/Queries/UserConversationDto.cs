using AutoMapper;
using talker.Application.Common.Mappings;
using talker.Domain.Entities;
using talker.Domain.Enums;

namespace talker.Application.Conversations.Queries
{
    public class UserConversationDto: IMapFrom<UserConversation>
    {
        public string UserId { get; set; }

        public ConversationRole Role { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserConversation, UserConversationDto>();
        }
    }
}
