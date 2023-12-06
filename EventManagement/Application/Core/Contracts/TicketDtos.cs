using System;

namespace EventManagement.Application.Core.Contracts;


public record GetAllMyTicketsResponse(Guid Id,Guid EventId, string EventName);
public record BookTicketRequest(Guid EventId);
public record CancelTicketRequest(Guid Id);
