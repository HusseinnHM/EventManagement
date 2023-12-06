using System;
using System.Linq;
using System.Linq.Expressions;
using EventManagement.Application.Core.Contracts;
using EventManagement.Entities;
using Neptunee.EntityFrameworkCore.MultiLanguage;

namespace EventManagement.Application.Events;

public static class EventStore
{
    public static Expression<Func<Event, GetAllEventResponse>> GetAllEventResponse(string languageKey)
        => e => new GetAllEventResponse(e.Id,
            e.Name.GetIn(languageKey),
            e.Description.GetIn(languageKey),
            e.Location.GetIn(languageKey),
            e.StartDate,
            e.EndDate,
            e.AvailableTickets,
            e.Tickets.Count);

    public static Expression<Func<Event, GetAvailableEventResponse>> GetAvailableEventResponse(string languageKey, Guid currentUserId)
        => e => new GetAvailableEventResponse(e.Id,
            e.Name.GetIn(languageKey),
            e.Description.GetIn(languageKey),
            e.Location.GetIn(languageKey),
            e.StartDate,
            e.EndDate,
            e.AvailableTickets - e.Tickets.Count,
            e.Tickets.Any(ep => ep.ParticipationUserId == currentUserId));
}