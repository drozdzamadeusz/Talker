using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using talker.Application.Updates.Queries.GetUpdates;
using Microsoft.AspNetCore.Authorization;
using talker.Application.Conversations.Queries.GetUserConversationsIds;
using talker.Application.Messages.Commands.SetMessagesAsSeen;

namespace talker.WebUI.Hubs
{
    [Authorize]
    public class UpdateHub : Hub
    {

        private readonly ILogger<UpdateHub> _logger;
        private readonly ISender _mediator;

        public UpdateHub(ILogger<UpdateHub> logger, ISender mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task SetAsSeen(int[] messagesIds, int conversationId)
        {

            var unseenMessages = await _mediator.Send(new SetMessagesAsSeenCommand
            {
                MessagesIds = messagesIds,
                ConversationId = conversationId,
                UserId  = Context.UserIdentifier,
            });

            if(unseenMessages > -1)
            {
                var update = new ConversationUpdateDto()
                {

                    Id = conversationId,
                    UnseenMessages = unseenMessages,
                    Type = Application.Updates.ConversationUpdateType.UserMarkedMessagesAsRead
                };

                await Clients.Caller.SendAsync("OnConversationUpdate", update);
            }
        }

        public void RemoveFromHub(int hubId)
        {
            _logger.LogWarning($"Removing client from chat hub: h-{hubId}");
            Groups.RemoveFromGroupAsync(Context.ConnectionId, $"h-{hubId}");
        }

        public async Task RemoveFromMyHubs()
        {
            var userConversationsIds = await _mediator.Send(new GetUserConversationsIdsQuery()
            {
                UserId = Context.UserIdentifier
            });

            if (userConversationsIds != null && userConversationsIds.Count > 0)
            {
                //_logger.LogWarning($"Removing client form chat hub groups: {JsonSerializer.Serialize(userConversationsIds)}");

                userConversationsIds.ForEach(c =>
                {
                    RemoveFromHub(c);
                });
            }
        }

        public void AddToHub(int hubId)
        {
            _logger.LogWarning($"Addng client to chat hub: h-{hubId}");
            Groups.AddToGroupAsync(Context.ConnectionId, $"h-{hubId}");
        }

        public async Task AddToMyHubsAsync()
        {

            var userConversationsIds = await _mediator.Send(new GetUserConversationsIdsQuery()
            {
                UserId = Context.UserIdentifier
            });


            if (userConversationsIds != null && userConversationsIds.Count > 0)
            {
                //_logger.LogWarning($"Addng client to chat hub groups: {JsonSerializer.Serialize(userConversationsIds)}");

                userConversationsIds.ForEach(c =>
                {
                    AddToHub(c);
                });

            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            await AddToMyHubsAsync();

            _logger.LogWarning($"Addng client to user hub: u-{Context.UserIdentifier}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"u-{Context.UserIdentifier}");

            _logger.LogWarning($"User connected to the chat room, ConnectionId: {Context.ConnectionId}, UserId: {Context.UserIdentifier}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            await RemoveFromMyHubs();

            _logger.LogWarning($"Removing client form user hub: u-{Context.UserIdentifier}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"u-{Context.UserIdentifier}");

            _logger.LogWarning($"A client disconnected from the chat room, ConnectionId: {Context.ConnectionId}, UserId: {Context.UserIdentifier}");
        }
    }
}
