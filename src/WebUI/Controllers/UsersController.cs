using talker.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using talker.WebUI.Controllers;
using System.Collections.Generic;
using talker.Application.Users.Queries.GetUsers;
using Microsoft.AspNetCore.SignalR;
using talker.WebUI.Hubs;
using talker.Application.Common.Interfaces;

namespace talker.WebUI.Controllers
{
    [Authorize]
    public class UsersController : ApiControllerBase
    {

        public UsersController(IHubContext<UpdateHub> hubContext, ICurrentUserService currentUserService) : base(hubContext, currentUserService)
        {
        }

        [HttpGet]
        public async Task<ActionResult<ApplicationUserDto>> GetUser([FromQuery] GetUserQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("List")]
        public async Task<ActionResult<List<ApplicationUserDto>>> GetUsers([FromQuery] GetUsersQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("Current")]
        public async Task<ActionResult<ApplicationUserDto>> GetCurrentUser()
        {
            return await Mediator.Send(new GetCurrentUserQuery());
        }

        [HttpGet("Find")]
        public async Task<ActionResult<List<ApplicationUserDto>>> FindUsers([FromQuery] FindUsersQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}
