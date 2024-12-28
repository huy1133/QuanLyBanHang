using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Shop.AppCodes;

namespace SV21T1020035.Shop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var userData = User.GetUserData();
            Customer? customerData;
            if (userData == null)
            {
                return RedirectToAction("Login");
            }
            customerData = CommomDataService.GetCustomer(int.Parse(userData.UserId));
            return View(customerData);
        }

        public IActionResult UpdateProfile(Customer data)
        {
            ModelState.Clear();
            if (string.IsNullOrWhiteSpace(data.CustomerName))
            {
                ModelState.AddModelError(nameof(data.CustomerName), "Vui lòng nhập tên khách hàng");
            }
            if (string.IsNullOrWhiteSpace(data.Address))
            {
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ");
            }
            if (string.IsNullOrWhiteSpace(data.Province))
            {
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");
            }
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            }
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập địa chỉ email");
            }
            if (!ModelState.IsValid)
            {
                return View("Profile", data);
            }
            bool result = CommomDataService.UpdateCustomer(data);
            if (!result)
            {
                ModelState.AddModelError(nameof(data.Email), "Địa chỉ email đã được sử dụng");
            }
            else
            {
                ModelState.AddModelError("Success", "Cập nhật thông tin thành công!");
            }
            return View("Profile", data);
        }

        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var customerData = User.GetUserData();
            if (Request.Method == "POST")
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Xác nhận mật khẩu không đúng");
                }
                if (string.IsNullOrWhiteSpace(oldPassword))
                {
					ModelState.AddModelError("oldPassword", "Vul lòng không để trống mật khẩu củ");
				}
				if (string.IsNullOrWhiteSpace(newPassword))
				{
					ModelState.AddModelError("newPassword", "Vul lòng không để trống mật khẩu mới");
				}
				if (customerData == null)
				{
					ModelState.AddModelError("confirmPassword", "Lỗi xát thực, vui lòng đăng xuất và đăng nhập lại");
				}
				if (ModelState.IsValid && customerData != null)
                {
					if (!UserAccountDataService.VerifyOldPassword(UserAccountDataService.TypeAccount.Customer, customerData.UserName, oldPassword))
					{
						ModelState.AddModelError("confirmPassword", "Mật khẩu củ không đúng");
                    }
                    else
                    {
						bool result = UserAccountDataService.ChangePassword(UserAccountDataService.TypeAccount.Customer, customerData.UserName, newPassword);
						if (result)
						{
							ModelState.AddModelError("Success", "Cập nhật mật khẩu thành công!");
						}
					}
				}
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.Title = "Đăng ký";
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewBag.Title = "Đăng nhập";
            return View();
        }
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(String username, String password)
        {
            ViewBag.Title = "Đăng nhập";
            ViewBag.Username = username;
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Erorr", "Nhập đầy đủ thông tin");
                return View();
            }
            CustomerAccount? account = UserAccountDataService.Authorize(UserAccountDataService.TypeAccount.Customer, username, password);
            if (account == null)
            {
                ModelState.AddModelError("Erorr", "Đăng nhập thất bại");
                return View();
            }
            var userData = new WebUserData()
            {
                UserId = account.CustomerId,
                UserName = account.CustomerName,
                DisplayName = account.DisplayName
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());
            return RedirectToAction("Index", "Home");
        }
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(String displayName, String email, String password, String confirmPassword)
        {
            ViewBag.DisplayName = displayName;
            ViewBag.Email = email;
            if (String.IsNullOrWhiteSpace(displayName))
            {
                ModelState.AddModelError("displayName", "Vui lòng nhập họ tên");
            }
            if (String.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("email", "Vui lòng nhập email");
            }
            if (String.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Vui lòng nhập mật khẩu");
            }
            if (confirmPassword != password)
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không chính xác");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            Customer customer = new Customer()
            {
                CustomerName = displayName,
                Email = email,
                Province = "Thừa Thiên Huế"
            };
            int result;
            result = CommomDataService.AddCustomer(customer);
            if (result < 0)
            {
                ModelState.AddModelError("email", "Email đã được sử dụng");
                return View();
            }
            else
            {
                bool rs = UserAccountDataService.ChangePassword(UserAccountDataService.TypeAccount.Customer,email,password);
                ModelState.AddModelError("Success", "Tạo tài khoản thành công!");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
