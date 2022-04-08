using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using talker.Application.Common.Interfaces;
using talker.Application.Conversations.Commands.AddUserToConversation;
using talker.Application.Conversations.Commands.CreateConversation;
using talker.Application.Conversations.Commands.LeaveConversation;
using talker.Application.Conversations.Commands.MakeUserAsAdmin;
using talker.Application.Conversations.Commands.MarkAsRead;
using talker.Application.Conversations.Commands.RemoveFromConversation;
using talker.Application.Conversations.Commands.RemoveUserAsAdmin;
using talker.Application.Conversations.Commands.UpdateConversation;
using talker.Application.Conversations.Queries;
using talker.Application.Conversations.Queries.GetConversation;
using talker.Application.Updates;
using talker.Application.Updates.Queries.GetUpdates;
using talker.WebUI.Hubs;

namespace talker.WebUI.Controllers
{
    [Authorize]
    public class ConversationsController : ApiControllerBase
    {

        public ConversationsController(IHubContext<UpdateHub> hubContext, ICurrentUserService currentUserService) : base(hubContext, currentUserService)
        {
        }


        [HttpGet]
        public async Task<ActionResult<List<ConversationDetailedDto>>> GetUserConversations()
        {
            return await Mediator.Send(new GetUserConversationsQuery());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConversationDto>> GetConversation(int id)
        {
            return await Mediator.Send(new GetConversationQuery
            {
                Id = id
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<int>> Update(int id, UpdateConversationCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(command);

            switch (result)
            {
                case 1:
                    await sendConversationEventMessage(id, Domain.Enums.MessageType.ConversationNameChanged, null, command.Name);

                    await _hubContext.Clients.Groups($"h-{id}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
                    {
                        Id = id,
                        Type = ConversationUpdateType.ConversationNameChanged,
                        Name = command.Name,
                    });
                    break;
                case 2:
                    await sendConversationEventMessage(id, Domain.Enums.MessageType.ConversationColorChanged, null, command.Color.ToString());

                    await _hubContext.Clients.Groups($"h-{id}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
                    {
                        Id = id,
                        Type = ConversationUpdateType.ConversationColorChanged,
                        Color = command.Color,
                    });

                    break;
                case 3:
                    await sendConversationEventMessage(id, Domain.Enums.MessageType.ConversationNameChanged, null, command.Name);
                    await sendConversationEventMessage(id, Domain.Enums.MessageType.ConversationColorChanged, null, command.Color.ToString());

                    await _hubContext.Clients.Groups($"h-{id}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
                    {
                        Id = id,
                        Type = ConversationUpdateType.ConversationNameChanged,
                        Name = command.Name,
                    });

                    await _hubContext.Clients.Groups($"h-{id}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
                    {
                        Id = id,
                        Type = ConversationUpdateType.ConversationColorChanged,
                        Color = command.Color,
                    });


                    break;

            }

            return result;
        }


        [HttpPost]
        public async Task<ActionResult<int>> CreateConversation(CreateConversationCommand command)
        {

            ActionResult<int> result = await Mediator.Send(command);

            command.UsersIds.ForEach(user =>
            {
                _hubContext.Clients.Groups($"u-{user.UserId}").SendAsync("OnUserUpdate", new UserUpdateDto
                {
                    ConversationId = (int)result.Value,
                    UserId = _currentUserService.UserId,
                    Type = UserUpdateType.AddedToConversation
                });
            });

            return result;
        }

        [HttpDelete("Leave/{id}")]
        public async Task<ActionResult> LeaveConversation(int id)
        {
            await Mediator.Send(new LeaveConversationCommand { Id = id });

            await _hubContext.Clients.Groups($"u-{_currentUserService.UserId}").SendAsync("OnUserUpdate", new UserUpdateDto
            {
                ConversationId = id,
                UserId = _currentUserService.UserId,
                Type = UserUpdateType.LeftConversation
            });

            await sendConversationEventMessage(id, Domain.Enums.MessageType.UserLeftConversation, null);

            await _hubContext.Clients.Groups($"h-{id}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
            {
                Id = id,
                Type = ConversationUpdateType.UserLeftConversation,
                UserId = _currentUserService.UserId
            });

            return NoContent();
        }

        [HttpDelete("RemoveUser")]
        public async Task<ActionResult> RemoveFromConversation([FromQuery] RemoveUserFromConversationCommand command)
        {
            await Mediator.Send(command);

            await sendConversationEventMessage(command.ConversationId, Domain.Enums.MessageType.RemovedUserFromConversation, command.UserId);

            await _hubContext.Clients.Groups($"u-{command.UserId}").SendAsync("OnUserUpdate", new UserUpdateDto
            {
                ConversationId = command.ConversationId,
                UserId = _currentUserService.UserId,
                Type = UserUpdateType.RemovedFromConversation
            });

            await _hubContext.Clients.Groups($"h-{command.ConversationId}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
            {
                Id = command.ConversationId,
                Type = ConversationUpdateType.UserRemovedFromConversation,
                UserId = command.UserId
            });

            return NoContent();
        }

        [HttpPut("MakeUserAsAdmin")]
        public async Task<ActionResult> MakeUserAsAdmin([FromQuery] MakeUserAsAdminCommand command)
        {
            await Mediator.Send(command);

            await sendConversationEventMessage(command.ConversationId, Domain.Enums.MessageType.AdminGranted, command.UserId);

            await _hubContext.Clients.Groups($"h-{command.ConversationId}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
            {
                Id = command.ConversationId,
                Type = ConversationUpdateType.AdminGranted,
                UserId = command.UserId
            });

            return NoContent();
        }

        [HttpDelete("RemoveUserAsAdmin")]
        public async Task<ActionResult> RemoveUserAsAdmin([FromQuery] RemoveUserAsAdminCommand command)
        {
            await Mediator.Send(command);

            await sendConversationEventMessage(command.ConversationId, Domain.Enums.MessageType.AdminRevoked, command.UserId);

            await _hubContext.Clients.Groups($"h-{command.ConversationId}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
            {
                Id = command.ConversationId,
                Type = ConversationUpdateType.AdminRevoked,
                UserId = command.UserId
            });

            return NoContent();
        }


        [HttpPut("AddUser")]
        public async Task<ActionResult> AddUserConversation([FromQuery] AddUserToConversationCommand command)
        {
            await Mediator.Send(command);

            await sendConversationEventMessage(command.ConversationId, Domain.Enums.MessageType.AddedUserToConversation, command.UserId);

            await _hubContext.Clients.Groups($"u-{command.UserId}").SendAsync("OnUserUpdate", new UserUpdateDto
            {
                ConversationId = command.ConversationId,
                UserId = _currentUserService.UserId,
                Type = UserUpdateType.AddedToConversation
            });

            await _hubContext.Clients.Groups($"h-{command.ConversationId}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
            {
                Id = command.ConversationId,
                Type = ConversationUpdateType.UserAddedToConversation,
                UserId = command.UserId
            });

            return NoContent();
        }

        [HttpPut("MarkAsRead/{id}")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            await Mediator.Send(new MarkAsReadCommand { Id = id });

            var update = new ConversationUpdateDto()
            {
                Id = id,
                UnseenMessages = 0,
                Type = ConversationUpdateType.UserMarkedMessagesAsRead
            };

            await _hubContext.Clients.Groups($"u-{_currentUserService.UserId}").SendAsync("OnConversationUpdate", update);

            return NoContent();
        }
    }
}
