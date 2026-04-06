using DevHabit.Api.Dtos.Common;
using Microsoft.AspNetCore.Http;

namespace DevHabit.Api.Service;

// encapsulates what's required to create LinkDto
public sealed class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
{
    public LinkDto Create(
        string endpointName,
        string rel,
        string method,
        object? values = null, // the values you can place in the route
        string? controller = null) // name of controller - useful if you want to reference action
                                   // that is outside the scope of current controller represented
                                   // by the current http context
    {
        string href = linkGenerator.GetUriByAction(
            httpContextAccessor.HttpContext!,
            endpointName,
            controller,
            values);

        return new LinkDto
        {
            Href = href ?? throw new Exception("Invalid endpoint name provided."),
            Rel = rel,
            Method = method,
        };
    } 
}
