using System;
using EventManagement.Common;
using Microsoft.AspNetCore.Authorization;

namespace EventManagement.Identity.Policies.UserType;

public class UserTypeRequirement : IAuthorizationRequirement
{
    public static string ClaimType => ConstValues.ClaimTypes.UserType;
        
    public Operator Operator { get; }
        
    public string[] UserTypes { get; }

    public UserTypeRequirement(Operator @operator, string[] userTypes)
    {
        if (userTypes.Length == 0)
            throw new ArgumentException("At least one UserType is required.", nameof(userTypes));

        Operator = @operator;
        UserTypes = userTypes;
    }
}