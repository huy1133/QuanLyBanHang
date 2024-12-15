using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;

namespace SV21T1020035.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(String username, String password)
        {
            ViewBag.Username = username;
            if (String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ thông tin");
                return View();
            }
            UserAccount? account = UserAccountDataService.Authorize(UserAccountDataService.TypeAccount.Employeer,username,password);
            if (account==null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
            //Tạo thông tin người dùng 
            var userData = new WebUserData
            {
                UserId = account.UserId,
                UserName = account.UserName,
                DisplayName = account.DisplayName,
                Photo = account.Photo,
                Roles = account.RoleName.Split(',').ToList(),
            };
            //ghi nhận trạng thái đăng nhập 
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());
            
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> LogoutAsync()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userData = User.GetUserData();
            if (Request.Method == "POST")
            {
                if (newPassword != confirmPassword) 
                {
                    ModelState.AddModelError("confirmPassword", "Xác nhận mật khẩu không đúng");
                }
                else if(userData == null) 
                {
                    ModelState.AddModelError("confirmPassword", "Lỗi xát thực, vui lòng đăng xuất và đăng nhập lại");
                }
                else if (!UserAccountDataService.VerifyOldPassword(UserAccountDataService.TypeAccount.Employeer, userData.UserName, oldPassword))
                {
                    ModelState.AddModelError("confirmPassword", "Mật khẩu củ không đúng");
                }
                else 
                {
                    bool result = UserAccountDataService.ChangePassword(UserAccountDataService.TypeAccount.Employeer, userData.UserName, newPassword);
                    if (result)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
