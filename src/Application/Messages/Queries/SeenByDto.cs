using AutoMapper;
using System;
using talker.Application.Common.Mappings;
using talker.Domain.Entities;

namespace talker.Application.Messages
{
    public class SeenByDto : IMapFrom<UserMessage>
    {
        public string UserId { get; set; }

        public DateTimeOffset Created { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserMessage, SeenByDto>();
        }
    }
}
