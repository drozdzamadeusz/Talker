using System;
using talker.Application.Common.Interfaces;

namespace talker.Application.Updates
{
    public class UserUpdateDto: IDateTime
    {
        public UserUpdateType Type { get; set; }
        public int ConversationId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; } = string.Empty;

        public DateTimeOffset Now => TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
    }
}
