using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.Shop.AppCodes;
using SV21T1020035.Shop.Models;
using System.Diagnostics;

namespace SV21T1020035.Shop.Controllers
{
    public class HomeController : Controller
    {
        private const int PAGE_SIZE = 32;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSerachCondition";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Trang chủ";
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput
                {
                    Page = 1,
                    SearchValue = string.Empty,
                    CategoryID = 0
                };
                //condition = new ProductSearchInput();
                ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            }
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page,PAGE_SIZE,condition.SearchValue,condition.CategoryID);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = PAGE_SIZE,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                CategoryID = condition.CategoryID,
                Data = data
            };
            return View(model);
        }

        public IActionResult Search(int categoryID=0, string searchValue="", int page=1)
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput
                {
                    Page = 1,
                    SearchValue = string.Empty,
                    CategoryID = 0
                };
            }
            if (page != 1)
            {
                condition.Page = page;
            }
            else
            {
                condition.SearchValue = searchValue;
                condition.CategoryID = categoryID;
                condition.Page = page;
            }
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return RedirectToAction("Index");
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
