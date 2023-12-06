using System;
using System.Collections.Generic;

namespace EventManagement.Application.Core.Contracts;

public record GetAllEventResponse(Guid Id, string Name, string Description, string Location, DateTime StartDate, DateTime EndDate, int AvailableTickets,int BookedTickets);

public record GetAvailableEventResponse(Guid Id, string Name, string Description, string Location, DateTime StartDate, DateTime EndDate, int AvailableTickets, bool AlreadyBook);

public record AddEventRequest(string Name, string Description, string Location, DateTime StartDate, DateTime EndDate, int AvailableTickets);

public record UpdateEventRequest(Guid Id, string Name, string Description, string Location, DateTime StartDate, DateTime EndDate, int AvailableTickets);

public record DeleteEventRequest(List<Guid> Ids);