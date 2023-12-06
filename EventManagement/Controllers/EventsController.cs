using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Core.Abstractions.Services;
using EventManagement.Application.Core.Contracts;
using EventManagement.Common;
using EventManagement.Common.ActionsFilters;
using EventManagement.Identity.Policies.UserType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neptunee.OperationResponse;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class EventsController : ControllerBase
{
    [HttpGet]
    [HasUserTypes(ConstValues.UserTypes.EventManager)]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<GetAllEventResponse>))]
    public async Task<IActionResult> GetAll(
        [FromServices] IEventService eventService)
        => await eventService.GetAll().ToIActionResultAsync(); // using pagination could optimize performance

    [HttpGet]
    [HasUserTypes(ConstValues.UserTypes.ParticipationUser)]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<GetAvailableEventResponse>))]
    public async Task<IActionResult> GetAvailable(
        [FromServices] IEventService eventService)
        => await eventService.GetAvailable().ToIActionResultAsync(); // using pagination could optimize performance

    [HttpPost]
    [HasUserTypes(ConstValues.UserTypes.EventManager)]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(GetAllEventResponse))]
    [TypeFilter(typeof(ForceDefaultLanguage))]
    public async Task<IActionResult> Add(
        [FromServices] IEventService eventService,
        [FromBody] AddEventRequest request)
        => await eventService.Add(request).ToIActionResultAsync();

    [HttpPost]
    [HasUserTypes(ConstValues.UserTypes.EventManager)]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(GetAllEventResponse))]
    public async Task<IActionResult> Update(
        [FromServices] IEventService eventService,
        [FromBody] UpdateEventRequest request)
        => await eventService.Update(request).ToIActionResultAsync();

    [HttpDelete]
    [HasUserTypes(ConstValues.UserTypes.EventManager)]
    public async Task<IActionResult> Delete(
        [FromServices] IEventService eventService,
        [FromBody] DeleteEventRequest request)
        => await eventService.Delete(request).ToIActionResultAsync();
}