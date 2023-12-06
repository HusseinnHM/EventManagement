using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using EventManagement.Application.Core.Abstractions.Services;
using EventManagement.Application.Core.Contracts;
using EventManagement.Common;
using EventManagement.Entities;
using EventManagement.Infrastructure.Jwt;
using EventManagement.Infrastructure.PasswordHash;
using EventManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Neptunee.BaseCleanArchitecture.Entities;
using Neptunee.OperationResponse;

namespace EventManagement.Application.EventManagers;

public class UserService : IUserService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRepository _repository;
    private readonly IJwtBearerGenerator _jwtBearerGenerator;

    public UserService(IPasswordHasher passwordHasher, IRepository repository, IJwtBearerGenerator jwtBearerGenerator)
    {
        _passwordHasher = passwordHasher;
        _repository = repository;
        _jwtBearerGenerator = jwtBearerGenerator;
    }

    public async Task<Operation<LoginUserResponse>> RegisterEventManager(RegisterUserRequest request)
    {
        return await Register(request.Email, () => new EventManager(request.Name, request.Email, _passwordHasher.HashPassword(request.Password)));
    }

    public async Task<Operation<LoginUserResponse>> RegisterParticipationUser(RegisterUserRequest request)
    {
        return await Register(request.Email, () => new ParticipationUser(request.Name, request.Email, _passwordHasher.HashPassword(request.Password)));
    }

    public async Task<Operation<LoginUserResponse>> Login<TUser>(LoginUserRequest request) where TUser : class, IEntityHasEmail, IEntityHasPassword, INeptuneeEntity<Guid>
    {
        var operation = Operation<LoginUserResponse>.Unknown();
        var user = await _repository
            .Query<TUser>()
            .Where(u => u.Email.ToUpper() == request.Email.ToUpper())
            .FirstOrDefaultAsync();
        if (user is null)
        {
            return operation.SetStatusCode(HttpStatusCode.NotFound).Error(Errors.Users.EmailNotFound);
        }

        if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password))
        {
            return operation.SetStatusCode(HttpStatusCode.NotFound).Error(Errors.Users.WrongEmailOrPassword);
        }

        return operation.SetResponse(SuccessResponse(user));
    }

    private async Task<Operation<LoginUserResponse>> Register<TUser>(string email, Func<TUser> userFunc) where TUser : class, IEntityHasEmail, IEntityHasPassword, INeptuneeEntity<Guid>
    {
        var operation = Operation<LoginUserResponse>.Unknown();

        if (await _repository.Query<TUser>().AnyAsync(u => u.Email.ToUpper() == email.ToUpper()))
        {
            return operation.SetStatusCode(HttpStatusCode.BadRequest).Error(Errors.Users.EmailAlreadyUsed);
        }

        var user = userFunc();
        _repository.Add(user);
        await _repository.UnitOfWork.SaveChangesAsync();
        return operation.SetResponse(SuccessResponse(user));
    }

    private LoginUserResponse SuccessResponse<TUser>(TUser user) where TUser : INeptuneeEntity<Guid>
        => new(_jwtBearerGenerator.Generate(new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ConstValues.ClaimTypes.UserType, typeof(TUser).Name),
            // other claim like expire, email ...etc
        }));
}