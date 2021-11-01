using talker.Application.Common.Models;
using talker.Application.TodoItems.Commands.CreateTodoItem;
using talker.Application.TodoItems.Commands.DeleteTodoItem;
using talker.Application.TodoItems.Commands.UpdateTodoItem;
using talker.Application.TodoItems.Commands.UpdateTodoItemDetail;
using talker.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using talker.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using talker.Application.Conversations.Commands.CreateConversation;
using talker.Application.Conversations.Queries.GetConversation;

namespace talker.WebUI.Controllers
{
    [Authorize]
    public class ConversationsController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ConversationDto>> GetConversation([FromQuery] GetConversationQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateConversation(CreateConversationCommand command)
        {
            return await Mediator.Send(command);
        }

        //[HttpPut("{id}")]
        //public async Task<ActionResult> Update(int id, UpdateTodoItemCommand command)
        //{
        //    if (id != command.Id)
        //    {
        //        return BadRequest();
        //    }

        //    await Mediator.Send(command);

        //    return NoContent();
        //}

        //[HttpPut("[action]")]
        //public async Task<ActionResult> UpdateItemDetails(int id, UpdateTodoItemDetailCommand command)
        //{
        //    if (id != command.Id)
        //    {
        //        return BadRequest();
        //    }

        //    await Mediator.Send(command);

        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    await Mediator.Send(new DeleteTodoItemCommand { Id = id });

        //    return NoContent();
        //}
    }
}
