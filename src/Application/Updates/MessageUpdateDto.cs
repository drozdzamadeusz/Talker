using System;
using System.Collections.Generic;
using talker.Application.Common.Interfaces;
using talker.Application.Messages;

namespace talker.Application.Updates.Queries.GetUpdates
{
    public class MessageUpdateDto : IDateTime
    {
        public MessageUpdateDto()
        {
            Messages = new List<MessageDto>();
        }

        public IList<MessageDto> Messages { get; set; } 

        public DateTimeOffset Now => TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
    }
}
