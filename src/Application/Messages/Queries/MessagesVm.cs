using System;
using System.Collections.Generic;
using talker.Application.Common.Interfaces;

namespace talker.Application.Messages.Queries
{
    public class MessagesVm: IDateTime
    {
        public MessagesVm()
        {
            Messages = new List<MessageDto>();
        }

        public IList<MessageDto> Messages { get; set; }

        public DateTimeOffset Now => TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
    }
}
