using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Core.Abstractions.Services;
using EventManagement.Application.Core.Contracts;
using EventManagement.Common;
using EventManagement.Identity.Policies.UserType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neptunee.OperationResponse;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TicketsController : ControllerBase
{
    [HttpGet]
    [HasUserTypes(ConstValues.UserTypes.ParticipationUser)]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<GetAllMyTicketsResponse>))]
    public async Task<IActionResult> GetAllMyTickets(
        [FromServices] ITicketService ticketService)
        => await ticketService.GetAllMyTickets().ToIActionResultAsync(); // using pagination could optimize performance
    
    [HttpPost]
    [HasUserTypes(ConstValues.UserTypes.ParticipationUser)]
    public async Task<IActionResult> Book(
        [FromServices] ITicketService ticketService,
        [FromBody] BookTicketRequest request)
        => await ticketService.Book(request).ToIActionResultAsync();  
    
    [HttpPost]
    [HasUserTypes(ConstValues.UserTypes.ParticipationUser)]
    public async Task<IActionResult> Cancel(
        [FromServices] ITicketService ticketService,
        [FromBody] CancelTicketRequest request)
        => await ticketService.Cancel(request).ToIActionResultAsync();   
}