using talker.Application.Common.Interfaces;
using System;

namespace talker.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
