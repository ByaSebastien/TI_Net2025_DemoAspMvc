using System.Security.Claims;
using TI_Net2025_DemoAspMvc.Models;

namespace TI_Net2025_DemoAspMvc.Extensions
{
    public static class UserExtensions
    {
        public static int GetId(this ClaimsPrincipal principal)
        {
            return int.Parse(principal.FindFirst(ClaimTypes.Sid)!.Value);
        }

        public static UserRole GetRole(this ClaimsPrincipal principal)
        {
            return Enum.Parse<UserRole>(principal.FindFirst(ClaimTypes.Role)!.Value);
        }

        public static bool IsConnected(this ClaimsPrincipal principal)
        {
            return principal.Identity!.IsAuthenticated;
        } 
    }
}
