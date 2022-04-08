using talker.Application.Common.Interfaces;
using System;

namespace talker.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTimeOffset Now => TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
    }
}
