using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using talker.Application.Common.Interfaces;
using talker.Application.Messages;
using talker.Application.Messages.Commands.SendEventMessage;
using talker.Application.Updates.Queries.GetUpdates;
using talker.Domain.Enums;
using talker.WebUI.Hubs;

namespace talker.WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private ISender _mediator;

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();

        protected readonly IHubContext<UpdateHub> _hubContext;
        protected readonly ICurrentUserService _currentUserService;

        protected ApiControllerBase(IHubContext<UpdateHub> hubContext, ICurrentUserService currentUserService)
        {
            _hubContext = hubContext;
            _currentUserService = currentUserService;
        }


        protected async Task sendConversationEventMessage(int conversationId, MessageType type, string userId = null, string value = null)
        {
            var result = await Mediator.Send(new SendEventMessageCommand
            {
                ConversationId = conversationId,
                Type = type,
                UserId = userId,
                Value = value,
            });

            var loggedUserId = _currentUserService.UserId;

            var update = new MessageUpdateDto()
            {
                Messages = new List<MessageDto>()
                    {
                        new MessageDto()
                        {
                            Id = result.Id,
                            ConversationId = conversationId,
                            Content = result.Content,
                            Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                            CreatedBy = loggedUserId,
                            Type = type
                        }
                    }
            };

            await _hubContext.Clients.Groups($"h-{conversationId}").SendAsync("OnMessageUpdate", update);
        }
    }
}
