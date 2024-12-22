using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;

namespace SV21T1020035.Shop.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult ProductDetail(int id)
        {
            var data = ProductDataService.GetProduct(id);
            return View(data);
        }
    }
}
