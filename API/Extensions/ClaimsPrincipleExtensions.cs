using System.Security.Claims;
using API.Errors;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var nameClaim = user.FindFirst(ClaimTypes.Name);
        if (nameClaim == null) throw new InvalidTokenException();
        return nameClaim.Value;
    }
    
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var nameClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (nameClaim == null) throw new InvalidTokenException();
        return int.Parse(nameClaim.Value);
    }
}