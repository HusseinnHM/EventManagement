using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventManagement.Application.Core.Abstractions.Services;
using EventManagement.Application.Core.Contracts;
using EventManagement.Common;
using EventManagement.Entities;
using EventManagement.Infrastructure.FakeTranslate;
using EventManagement.Infrastructure.HttpResolver;
using EventManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Neptunee.OperationResponse;

namespace EventManagement.Application.Events;

public class EventService : IEventService
{
    private readonly IRepository _repository;
    private readonly IHttpResolver _httpResolver;
    private readonly IFakeTranslate _fakeTranslate;

    public EventService(IRepository repository, IHttpResolver httpResolver, IFakeTranslate fakeTranslate)
    {
        _repository = repository;
        _httpResolver = httpResolver;
        _fakeTranslate = fakeTranslate;
    }

    public async Task<Operation<List<GetAllEventResponse>>> GetAll()
    {
        var currentUserId = _httpResolver.CurrentUserId();
        return await _repository
            .Query<Event>()
            .Where(e => e.EventManagerId == currentUserId)
            .OrderBy(e => e.StartDate <= DateTime.Now)
            .Select(EventStore.GetAllEventResponse(_httpResolver.LanguageKey()))
            .ToListAsync();
    }

    public async Task<Operation<List<GetAvailableEventResponse>>> GetAvailable()
    {
        return await _repository
            .Query<Event>()
            .Where(e => e.StartDate >= DateTime.Now && e.AvailableTickets > e.Tickets.Count)
            .OrderBy(e => e.StartDate)
            .Select(EventStore.GetAvailableEventResponse(_httpResolver.LanguageKey(), _httpResolver.CurrentUserId()))
            .ToListAsync();
    }

    public async Task<Operation<GetAllEventResponse>> Add(AddEventRequest request)
    {
        var eventEntity = new Event(request.Name,
            request.Description,
            request.Location,
            request.StartDate,
            request.EndDate,
            request.AvailableTickets,
            _httpResolver.CurrentUserId(),
            _httpResolver.LanguageKey());
        _fakeTranslate.Translate(eventEntity.Name, eventEntity.Description, eventEntity.Location);
        _repository.Add(eventEntity);
        await _repository.UnitOfWork.SaveChangesAsync();
        return await _repository.GetFirstAsync(eventEntity.Id, EventStore.GetAllEventResponse(_httpResolver.LanguageKey()));
    }


    public async Task<Operation<GetAllEventResponse>> Update(UpdateEventRequest request)
    {
        var operation = Operation<GetAllEventResponse>.Unknown();

        operation.OrFailureIf(request.StartDate < DateTime.Now, Errors.Events.StartDateCannotBeInPast);
        operation.OrFailureIf(request.EndDate < request.StartDate, Errors.Events.EndDateCannotBeBeforeStatDate);
        // other validations checks ..

        if (operation.IsFailure)
        {
            return operation;
        }

        var eventEntity = await _repository.TrackingQuery<Event>().Where(e => e.Id == request.Id).Include(e => e.Tickets).FirstOrDefaultAsync();
        if (eventEntity is null)
        {
            return operation.SetStatusCode(HttpStatusCode.NotFound);
        }

        if (eventEntity.EventManagerId != _httpResolver.CurrentUserId())
        {
            return operation.SetStatusCode(HttpStatusCode.Forbidden);
        }

        var result = eventEntity.Modify(request.Name,
            request.Description,
            request.Location,
            request.StartDate,
            request.EndDate,
            request.AvailableTickets,
            _httpResolver.LanguageKey());
        if (result.IsFailure)
        {
            return result;
        }

        // await Task.Delay(10000); to testing Concurrency
        try
        {
            await _repository.UnitOfWork.SaveChangesAsync();
            operation.SetResponse(await _repository.GetFirstAsync(eventEntity.Id, EventStore.GetAllEventResponse(_httpResolver.LanguageKey())));
        }
        catch (DbUpdateConcurrencyException)
        {
            operation.Error(Errors.Events.ChangesHappened);
        }

        return operation;
    }

    public async Task<Operation<GetAllEventResponse>> Update2Approach(UpdateEventRequest request)
    {
        Event? eventEntity = null;
        return await Operation<GetAllEventResponse>
            .FailureIf(request.StartDate < DateTime.Now, Errors.Events.StartDateCannotBeInPast)
            .OrFailureIf(request.EndDate < request.StartDate, Errors.Events.EndDateCannotBeBeforeStatDate)
            .OnSuccessAsync(async _ =>
                eventEntity = await _repository
                    .TrackingQuery<Event>()
                    .Where(e => e.Id == request.Id)
                    .Include(e => e.Tickets)
                    .FirstOrDefaultAsync())
            .AndFailureIf(() => eventEntity is null, operation => operation.SetStatusCode(HttpStatusCode.NotFound))
            .AndFailureIf(() => eventEntity!.EventManagerId != _httpResolver.CurrentUserId(), operation => operation.SetStatusCode(HttpStatusCode.Forbidden))
            .AndIf(() => eventEntity!.Modify(request.Name,
                request.Description,
                request.Location,
                request.StartDate,
                request.EndDate,
                request.AvailableTickets,
                _httpResolver.LanguageKey()))
            .OnSuccessAsync(async operation =>
            {
                try
                {
                    await _repository.UnitOfWork.SaveChangesAsync();
                    operation.SetResponse(await _repository.GetFirstAsync(eventEntity!.Id, EventStore.GetAllEventResponse(_httpResolver.LanguageKey())));
                }
                catch (DbUpdateConcurrencyException)
                {
                    operation.Error(Errors.Events.ChangesHappened);
                }
            });
    }

    public async Task<Operation<NoResponse>> Delete(DeleteEventRequest request)
    {
        if (await _repository.Query<Event>().AnyAsync(e => request.Ids.Contains(e.Id) && e.EventManagerId != _httpResolver.CurrentUserId()))
        {
            return Operation<NoResponse>.Unknown().SetStatusCode(HttpStatusCode.Forbidden);
        }

        await _repository.TrackingQuery<Event>().Where(e => request.Ids.Contains(e.Id)).ExecuteDeleteAsync();
        return Operation<NoResponse>.Ok();
    }
}