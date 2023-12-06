using System.Threading.Tasks;
using EventManagement.Application.Core.Abstractions.Services;
using EventManagement.Application.Core.Contracts;
using EventManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neptunee.OperationResponse;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class EventManagersController : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(LoginUserResponse))]
    public async Task<IActionResult> Register(
        [FromServices] IUserService userService,
        [FromBody] RegisterUserRequest request)
        => await userService.RegisterEventManager(request).ToIActionResultAsync();

    [HttpPost]
    [AllowAnonymous]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(LoginUserResponse))]
    public async Task<IActionResult> Login(
        [FromServices] IUserService userService,
        [FromBody] LoginUserRequest request)
        => await userService.Login<EventManager>(request).ToIActionResultAsync();
}