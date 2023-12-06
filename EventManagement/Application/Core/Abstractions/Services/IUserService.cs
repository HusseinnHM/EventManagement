using System;
using System.Threading.Tasks;
using EventManagement.Application.Core.Contracts;
using EventManagement.Entities;
using Neptunee.BaseCleanArchitecture.Entities;
using Neptunee.OperationResponse;

namespace EventManagement.Application.Core.Abstractions.Services;

public interface IUserService : IApplicationService
{
    Task<Operation<LoginUserResponse>> RegisterEventManager(RegisterUserRequest request);
    Task<Operation<LoginUserResponse>> RegisterParticipationUser(RegisterUserRequest request);
    Task<Operation<LoginUserResponse>> Login<TUser>(LoginUserRequest request) where TUser : class, IEntityHasEmail, IEntityHasPassword, INeptuneeEntity<Guid>;
}