using DevHabit.Api.Dtos.Common;

namespace DevHabit.Api.Services;

public sealed class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
{
    public LinkDto Create(
        string endpointName,
        string rel,
        string method,
        object? values = null,// the values that can be placed in the route
        string? controller = null)// useful if i need to ref an action that is outside of the current controller
    {
        string? href = linkGenerator.GetUriByAction(httpContextAccessor.HttpContext!, endpointName, controller, values);

        return new LinkDto
        {
            Href = href ?? throw new Exception("Invalid endpoint name provided."),
            Method = method,
            Rel = rel,
        };
    }
}
