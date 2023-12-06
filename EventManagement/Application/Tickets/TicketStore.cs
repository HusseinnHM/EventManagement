using System;
using System.Linq.Expressions;
using EventManagement.Application.Core.Contracts;
using EventManagement.Entities;
using Neptunee.EntityFrameworkCore.MultiLanguage;

namespace EventManagement.Application.Tickets;

public class TicketStore
{
    public static Expression<Func<Ticket, GetAllMyTicketsResponse>> GetMyTicketsResponse(string languageKey)
        => e => new GetAllMyTicketsResponse(e.Id,
            e.EventId,
            e.Event.Name.GetIn(languageKey));
}