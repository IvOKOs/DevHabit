using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace DevHabit.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetIdentityId(this ClaimsPrincipal? claimsPrincipal)
    {
        string? identityUserId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
        return identityUserId;
    }
}
