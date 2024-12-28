using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020035.Shop.AppCodes
{
    public class WebUserData
    {
        public String UserId { get; set; } = "";
        public String UserName { get; set; } = "";
        public String DisplayName { get; set; } = "";

        public ClaimsPrincipal CreatePrincipal()
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(nameof(UserId), UserId),
                new Claim(nameof(UserName), UserName),
                new Claim(nameof(DisplayName), DisplayName),
            };
            
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
