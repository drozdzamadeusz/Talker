using System;

namespace talker.Application.Common.Interfaces
{
    public interface IDateTime
    {
        DateTimeOffset Now { get; }
    }
}
