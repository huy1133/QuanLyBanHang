using System.Security.Claims;

namespace SV21T1020035.Shop.AppCodes
{
    public static class WebUserExtension
    {
        public static WebUserData? GetUserData(this ClaimsPrincipal principal)
        {
            try
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                {
                    return null;
                }
                WebUserData? userData = new WebUserData();
                userData.UserId = principal.FindFirstValue(nameof(userData.UserId))??"";
                userData.UserName = principal.FindFirstValue(nameof(userData.UserName)) ?? "";
                userData.DisplayName = principal.FindFirstValue(nameof(userData.DisplayName)) ?? "";
                return userData;
            }
            catch
            {
                return null;
            }
        }
    }
}
