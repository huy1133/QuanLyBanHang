using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Shop.AppCodes;
using SV21T1020035.Shop.Models;

namespace SV21T1020035.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string SHOPPING_CART = "ShoppingCart";
        private const String ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public IActionResult Index()
        {
            OrderSearchInput? condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if(condition == null)
            {
                DateTime currentDate = DateTime.Now;
                DateTime pastDate = currentDate.AddDays(-15);
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    TimeRange = $"{pastDate:dd/MM/yyyy} - {currentDate:dd/MM/yyyy}"
                };
            }
            return View(condition);
        }

        public IActionResult Search(OrderSearchInput condition)
        {
            WebUserData? customerData = User.GetUserData();
            int customerID = 0;
            if (customerData != null)
            {
                customerID = int.Parse(customerData.UserId);
            }
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status,
                condition.FromTime, condition.ToTime, condition.SearchValue ?? "", customerID);
            OrderSearchResult model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data,
                Status = condition.Status,
                TimeRange = condition.TimeRange
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Create()
        {
            string deliveryProvince = "";
            string deliveryAddress = "";
            var shoppingCart = GetShoppingCart();
            WebUserData? customerData = User.GetUserData();
            if (shoppingCart.Count == 0)
            {
                return Json("Không có mặt hàng để đặt hàng!");
            }

            int employeeID = 1;
            int customerID = 1;
            if(customerData != null)
            {
                customerID = int.Parse(customerData.UserId);
                Customer? customer = CommomDataService.GetCustomer(customerID);
                deliveryProvince = customer?.Province??"";
                deliveryAddress = customer?.Address??"";
            }
            
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json("");
        }

        public IActionResult OrderDetail(int id)
        {
            Order? order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            List<OrderDetail> Details = OrderDataService.ListOrderDetails(id);

            OrderDetailModel model = new OrderDetailModel
            {
                Order = order,
                Details = Details
            };
            return View(model);
        }

        public IActionResult Cancel(int id)
        {
            bool resultt = OrderDataService.CancelOrder(id);
            return RedirectToAction("OrderDetail", new { id = id });
        }

        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
    }
}
