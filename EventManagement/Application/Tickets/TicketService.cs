using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventManagement.Application.Core.Abstractions.Services;
using EventManagement.Application.Core.Contracts;
using EventManagement.Entities;
using EventManagement.Infrastructure.HttpResolver;
using EventManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Neptunee.OperationResponse;

namespace EventManagement.Application.Tickets;

public class TicketService : ITicketService
{
    private readonly IRepository _repository;
    private readonly IHttpResolver _httpResolver;


    public TicketService(IRepository repository, IHttpResolver httpResolver)
    {
        _repository = repository;
        _httpResolver = httpResolver;
    }

    public async Task<Operation<List<GetAllMyTicketsResponse>>> GetAllMyTickets()
    {
        var currentUserId = _httpResolver.CurrentUserId();
        return await _repository
            .Query<Ticket>()
            .Where(t => t.ParticipationUserId == currentUserId)
            .OrderBy(t => t.Event.StartDate <= DateTime.Now)
            .Select(TicketStore.GetMyTicketsResponse(_httpResolver.LanguageKey()))
            .ToListAsync();
    }

    public async Task<Operation<NoResponse>> Book(BookTicketRequest request)
    {
        var eventEntity = await _repository
            .TrackingQuery<Event>()
            .Where(e => e.Id == request.EventId)
            .Include(e => e.Tickets)
            .FirstAsync();
        var result = eventEntity.Book(_httpResolver.CurrentUserId());
        if (result.IsFailure)
        {
            return result;
        }

        // await Task.Delay(10000); to testing Concurrency
        try
        {
            await _repository.UnitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return await Book(request);
        }

        return Operation<NoResponse>.Ok();
    }

    public async Task<Operation<NoResponse>> Cancel(CancelTicketRequest request)
    {
        var operation = Operation<NoResponse>.Unknown();
        var currentUserId = _httpResolver.CurrentUserId();
        var deletedRows = await _repository
            .TrackingQuery<Ticket>()
            .Where(t => t.Id == request.Id && t.ParticipationUserId == currentUserId)
            .ExecuteDeleteAsync();
        return deletedRows == 0
            ? operation.SetStatusCode(HttpStatusCode.NotFound)
            : operation.SetStatusCode(HttpStatusCode.OK);
    }


}