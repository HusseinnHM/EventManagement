using System;
using Neptunee.BaseCleanArchitecture.Entities;

namespace EventManagement.Entities;

public class Entity : INeptuneeEntity<Guid>
{
    public Entity()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get;  set; }
}

public interface IEntityHasEmail
{
    public string Email { get; }
}public interface IEntityHasPassword
{
    public string PasswordHash { get; }
}