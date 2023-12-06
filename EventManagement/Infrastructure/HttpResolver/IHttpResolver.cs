using System;

namespace EventManagement.Infrastructure.HttpResolver;

public interface IHttpResolver
{
    Guid CurrentUserId();
    string LanguageKey();
    bool IsDefaultLanguageKey();
}