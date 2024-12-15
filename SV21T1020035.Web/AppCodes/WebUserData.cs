using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020035.Web
{
    /// <summary>
    /// Những thông tin cần thể hiện trong danh tính của người dùng 
    /// </summary>
    public class WebUserData
    {
        public String UserId { get; set; } = "";
        public String UserName { get; set; } = "";
        public String DisplayName { get; set; } = "";
        public String Photo { get; set; } = "";
        public List<String>? Roles { get; set; }
        /// <summary>
        /// Tạo ra chứng chận ghi nhận danh tính của người dùng
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal()
        {
            //Danh sách cac Claim chứa các thông tin liên quan đến danh tính người dùng 
            List<Claim> claims = new List<Claim>() {
                new Claim(nameof(UserId), UserId),
                new Claim(nameof(UserName), UserName),
                new Claim(nameof(DisplayName), DisplayName),
                new Claim(nameof(Photo), Photo)
            };
            if (Roles != null)
            {
                foreach(var role in Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            //Tạo identity dựa trên các thông tin có trong danh sách các claim
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //Tạo principal 
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
