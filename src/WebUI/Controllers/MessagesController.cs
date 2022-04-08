using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Common.Interfaces;
using talker.Application.Common.Models;
using talker.Application.Messages;
using talker.Application.Messages.Commands.SendMessage;
using talker.Application.Messages.Commands.SetMessagesAsSeen;
using talker.Application.Messages.Queries;
using talker.Application.Messages.Queries.GetMessage;
using talker.Application.Messages.Queries.GetMessages;
using talker.Application.Messages.Queries.GetMessagesWithPagination;
using talker.Application.Updates.Queries.GetUpdates;
using talker.Infrastructure.Identity;
using talker.WebUI.Hubs;

namespace talker.WebUI.Controllers
{
    [Authorize]
    public class MessagesController : ApiControllerBase
    {

        public MessagesController(IHubContext<UpdateHub> hubContext, ICurrentUserService currentUserService) : base(hubContext, currentUserService)
        {
        }

        [HttpGet("GetWithPagination")]
        public async Task<ActionResult<PaginatedList<MessageDto>>> GetMessagesWithPagination([FromQuery] GetMessagesWithPaginationQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet]
        public async Task<ActionResult<MessagesVm>> GetMessages([FromQuery] GetMessagesQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDto>> GetMessage(int id)
        {
            return await Mediator.Send(new GetMessageQuery {
                Id = id,
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> SendMessage(SendMessageCommand command)
        {
            ActionResult<int> result;

            try
            {
                result = await Mediator.Send(command);

                var userId = _currentUserService.UserId;

                var update = new MessageUpdateDto()
                {
                    Messages = new List<MessageDto>()
                    {
                        new MessageDto()
                        {
                            Id = result.Value,
                            ConversationId = command.ConversationId,
                            Content = command.Content,
                            Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                            CreatedBy = userId,
                        }
                    }
                };

                await _hubContext.Clients.Groups($"h-{command.ConversationId}").SendAsync("OnMessageUpdate", update);

            }
            catch (InvalidOperation ex)
            {
                return BadRequest(new ExceptionResult()
                {
                    Errors = (Dictionary<string, string[]>)ex.Errors
                });
            }

            return result;
        }

        [HttpPut("SetAsSeen")]
        public async Task<ActionResult<int>> SetAsSeen(SetMessagesAsSeenCommand command)
        {
            return await Mediator.Send(command);
        }
    }
}
