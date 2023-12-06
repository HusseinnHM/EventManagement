using System.Collections.Generic;
using System.Security.Claims;

namespace EventManagement.Infrastructure.Jwt;

public interface IJwtBearerGenerator
{
    string Generate(List<Claim> claims);

}