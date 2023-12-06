using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Core.Contracts;
using Neptunee.OperationResponse;

namespace EventManagement.Application.Core.Abstractions.Services;

public interface IEventService : IApplicationService
{
    Task<Operation<List<GetAllEventResponse>>> GetAll();
    Task<Operation<List<GetAvailableEventResponse>>> GetAvailable();
    Task<Operation<GetAllEventResponse>> Add(AddEventRequest request);
    Task<Operation<GetAllEventResponse>> Update(UpdateEventRequest request);
    Task<Operation<NoResponse>> Delete(DeleteEventRequest request);
}