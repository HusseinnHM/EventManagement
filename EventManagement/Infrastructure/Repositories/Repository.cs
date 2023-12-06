using System;
using EventManagement.Infrastructure.EntityFrameworkCore.DbContexts;
using Neptunee.BaseCleanArchitecture.Repository;

namespace EventManagement.Infrastructure.Repositories;

public interface IRepository : INeptuneeRepository<Guid>
{
    
}

public class Repository(EventManagementDbContext context) : NeptuneeRepository<Guid, EventManagementDbContext>(context), IRepository;