using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Core.Contracts;
using Neptunee.OperationResponse;

namespace EventManagement.Application.Core.Abstractions.Services;

public interface ITicketService : IApplicationService
{
    Task<Operation<List<GetAllMyTicketsResponse>>> GetAllMyTickets();
    
    Task<Operation<NoResponse>> Book(BookTicketRequest request);
    Task<Operation<NoResponse>> Cancel(CancelTicketRequest request);
}