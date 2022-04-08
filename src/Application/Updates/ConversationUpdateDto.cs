using System;
using talker.Application.Common.Interfaces;
using talker.Application.Conversations.Queries;
using talker.Domain.Enums;

namespace talker.Application.Updates.Queries.GetUpdates
{
    public class ConversationUpdateDto : IDateTime
    {
        public ConversationUpdateType Type { get; set; }

        public int Id { get; set; }

        public int? UnseenMessages { get; set; }

        public string? UserId { get; set; }

        public string? Name { get; set; }

        public ConversationColor? Color { get; set; }

        public DateTimeOffset Now => TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
    }
}
