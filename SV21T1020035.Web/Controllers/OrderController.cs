using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Web.Models;

namespace SV21T1020035.Web.Controllers
{
    [Authorize(Roles = $"{UserRoles.ADMIN},{UserRoles.EMPLOYEER}")]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 30;
        private const String ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        private const int PRODUCT_SEARCH_PAGE_SIZE = 5;
        private const String PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        private const String SHOPPING_CART = "ShoppingCart";
        public IActionResult Index()
        {
            OrderSearchInput? condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = ""
                };
            }
            return View(condition);
        }
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status,
                condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
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

        public IActionResult Details(int id = 0)
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
        public IActionResult Delete(int id)
        {
            bool result = OrderDataService.DeleteOrder(id);
            return RedirectToAction("Index");
        }
        public IActionResult Accept(int id)
        {
            bool resultt = OrderDataService.AcceptOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Cancel(int id)
        {
            bool resultt = OrderDataService.CancelOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Reject(int id)
        {
            bool resultt = OrderDataService.RejectOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Finish(int id)
        {
            bool resultt = OrderDataService.FinishOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var data = OrderDataService.GetOrderDetail(id,productId);
            return View(data);
        }
		[HttpPost]
		public IActionResult SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
		{
            if (salePrice < 0||quantity<1)
            {
                return Json("Vui lòng nhập giá và số lượng hợp lệ!");
            }
			bool result = OrderDataService.SaveOrderDetail(orderID, productID, quantity, salePrice);
			if (!result)
			{
				return Json("Phát sinh lỗi trong quá trình cập nhật!");
            }
            return Json(orderID);
        }

        public IActionResult DeleteDetail(int id, int productId)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productId);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (Request.Method == "POST")
            {
                if (shipperID == 0)
                {
                    return Json("Vui lòng chọn giao hàng!");
                }
                else
                {
                    bool result = OrderDataService.ShipOrder(id, shipperID);
                    return Json(id);
                }
            }
            return View(id);
        }

        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput
                {
                    Page = 1,
                    PageSize = PRODUCT_SEARCH_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }

        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue);
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize=condition.PageSize,
                SearchValue = condition.SearchValue,
                Data = data,
                RowCount = rowCount
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }

        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if(shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        public IActionResult AddToCart(CartItem item)
        {
            if(item.Quantity<=0||item.SalePrice < 0)
            {
                return Json("Giá bán và số lượng không hợp lệ");
            }
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice += item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromcart(int id)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m=>m.ProductID == id);
            if (index >= 0)
            {
                shoppingCart.RemoveAt(index);
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        [HttpPost]
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
            {
                return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán.");
            }
            if (customerID == 0 || string.IsNullOrEmpty(deliveryAddress) || string.IsNullOrEmpty(deliveryProvince))
            {
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng.");
            }
            var userData = User.GetUserData();
            if (userData == null)
            {
                return Json("Lỗi xác thực thông tin. Vui lòng đăng xuất và đăng nhập lại");
            }
            int employeeID = int.Parse(userData.UserId);
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
            return Json(orderID);
        }

    }
}

