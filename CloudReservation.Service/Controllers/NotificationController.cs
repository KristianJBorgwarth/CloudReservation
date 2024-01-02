using CloudReservation.Service.Commands.NotificationCommands.EventNotification;
using CloudReservation.Service.Models.NotificationModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using Newtonsoft.Json;

namespace CloudReservation.Service.Controllers;

[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediator _mediator;

    public NotificationController(IHttpContextAccessor httpContextAccessor, IMediator mediator)
    {
        _httpContextAccessor = httpContextAccessor;
        _mediator = mediator;
    }

    [HttpPost("listen-second")]
    public async Task<ActionResult> SingleListen()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context.Request.Query.ContainsKey("validationToken"))
        {
            var token = context.Request.Query["validationToken"].ToString();
            return Content(token, "text/plain");
        }
        else
        {
            var reqBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var response = JsonConvert.DeserializeObject<NotificationWrapper>(reqBody);

            var result = await _mediator.Send(new EventNotificationCommand()
            {
                NotificationWrapper = response
            });
        }

        return Accepted();
    }
}

