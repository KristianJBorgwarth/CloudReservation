using System.Security.Claims;

namespace CloudReservation.Service.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email)?.ToLower();
    }
}