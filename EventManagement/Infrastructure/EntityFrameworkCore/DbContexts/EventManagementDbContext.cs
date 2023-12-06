using System;
using System.Linq;
using System.Reflection;
using EventManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Neptunee.BaseCleanArchitecture.Clock;
using Neptunee.BaseCleanArchitecture.DbContexts;
using Neptunee.BaseCleanArchitecture.Dispatchers.DomainEventDispatcher;
using Neptunee.EntityFrameworkCore.MultiLanguage.Extensions;

namespace EventManagement.Infrastructure.EntityFrameworkCore.DbContexts;

public class EventManagementDbContext : NeptuneeDbContext<Guid>
{
    public EventManagementDbContext(DbContextOptions<EventManagementDbContext> options, INeptuneeClock clock, INeptuneeDomainEventDispatcher domainEventDispatcher) : base(options, clock, domainEventDispatcher)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<EventManager> EventManagers => Set<EventManager>();
    public DbSet<ParticipationUser> ParticipationUsers => Set<ParticipationUser>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.SetPrimaryKeyValueGenerated(ValueGenerated.Never);
        builder.ConfigureMultiLanguage(Database);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), t => t.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));
    }
}