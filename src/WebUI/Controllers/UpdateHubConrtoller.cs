using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace talker.WebUI.Controllers
{

    public class UpdateController { //: UpdateHub{

        //private readonly ISender _mediator;

        //public UpdateController(ISender mediator)
        //{
        //    _mediator = mediator;
        //}

        //public async Task Update()//[FromQuery] GetUpdatesQuery query)
        //{
        //    do
        //    {
        //       // var updates  = await _mediator.Send(query);

        //        await Task.Delay(2000);

        //        //if (updates == null || updates.Messages.Count < 1) continue;

        //        await Clients.Caller.SendAsync("updates", "aaa");

        //    } while(true);

        //}

    }
}
