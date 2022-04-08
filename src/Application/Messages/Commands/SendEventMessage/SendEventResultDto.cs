using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talker.Domain.Enums;

namespace talker.Application.Messages.Commands.SendEventMessage
{
    public class SendEventResultDto
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public MessageType Type { get; set; }
    }
}
