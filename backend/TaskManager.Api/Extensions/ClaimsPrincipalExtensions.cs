using System.Security.Claims;

namespace TaskManager.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("User id claim not found.");

        return Guid.Parse(value);
    }
}
