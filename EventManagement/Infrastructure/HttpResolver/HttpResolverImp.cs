using System;
using System.Security.Claims;
using EventManagement.Common;
using EventManagement.Common.LanguageKey;
using EventManagement.Infrastructure.HttpResolver.Exceptions;
using Microsoft.AspNetCore.Http;

namespace EventManagement.Infrastructure.HttpResolver;

public class HttpResolverImp : IHttpResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;



    public HttpResolverImp(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid CurrentUserId()
    {
        return Guid.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : Guid.Empty;
    }

    public string LanguageKey()
    {
        var languageKey = _httpContextAccessor.HttpContext?.Request.Headers[ConstValues.HeaderKeys.Language].ToString() ?? string.Empty;

        return LanguageKeys.Validate(languageKey)
            ? languageKey
            : throw new InvalidHeaderException(ConstValues.HeaderKeys.Language);
    }

    public bool IsDefaultLanguageKey()
    {
        return LanguageKeys.Default.Equals(LanguageKey(), StringComparison.OrdinalIgnoreCase);
    }
}