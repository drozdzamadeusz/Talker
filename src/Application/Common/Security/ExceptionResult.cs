using System;
using System.Collections.Generic;

namespace talker.Infrastructure.Identity
{
    public class ExceptionResult
    {
        public string Type { get; set; } = "Unknown type";

        public string Title { get; set; } = "Unknown exception";

        public int Status { get; set; }

        public Dictionary<string, string[]> Errors { get; set; }


    }
}
