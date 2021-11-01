using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using talker.Application.Common.Exceptions;
using talker.Application.Messages.Commands.SendMessage;
using talker.Infrastructure.Identity;

namespace talker.WebUI.Controllers
{
    [Authorize]
    public class MessagesController : ApiControllerBase
    {
        //[HttpGet]
        //public async Task<ActionResult<ConversationDto>> GetConversation([FromQuery] GetConversationQuery query)
        //{
        //    return await Mediator.Send(query);
        //}

        [HttpPost]
        public async Task<ActionResult<int>> SendMessage(SendMessageCommand command)
        {
            try
            {
                return await Mediator.Send(command);
            }
            catch (InvalidOperation ex)
            {
                return BadRequest(new ExceptionResult()
                {
                    Errors = (System.Collections.Generic.Dictionary<string, string[]>)ex.Errors
                });
            }
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
