using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.Shop.AppCodes;

namespace SV21T1020035.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {

            HttpContext.Session.Clear();
            return Json("");
        }
    }
}
