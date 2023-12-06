using EventManagement.Common.LanguageKey;
using EventManagement.Infrastructure.HttpResolver;
using Microsoft.AspNetCore.Mvc.Filters;
using Neptunee.OperationResponse;

namespace EventManagement.Common.ActionsFilters;

public class ForceDefaultLanguage : IActionFilter
{
    private readonly IHttpResolver _httpResolver;

    public ForceDefaultLanguage(IHttpResolver httpResolver)
    {
        _httpResolver = httpResolver;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!_httpResolver.IsDefaultLanguageKey())
        {
            context.Result = Operation<NoResponse>.BadRequest().Error($"{ConstValues.HeaderKeys.Language} header value must be : {LanguageKeys.Default} (default)").ToIActionResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
       
    }
}