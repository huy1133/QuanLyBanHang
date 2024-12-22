using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Shop.AppCodes;
using SV21T1020035.Shop.Models;

namespace SV21T1020035.Shop.Controllers
{
    public class CartController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
        public IActionResult Index()
        {
            var shoppingCart = GetShoppingCart();
            decimal sumTotal = 0;
            int count = 0;
            shoppingCart.ForEach(shoppingCartItem => { sumTotal += shoppingCartItem.TotalPrice; count++; });
            ViewBag.Count= count;
            ViewBag.SumTotal = sumTotal;
            return View(shoppingCart);
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
        public IActionResult AddToCart(int productID, int quantity)
        {
            Product? product = ProductDataService.GetProduct(productID);
            if (product == null)
                return Json("Sản phẩm không tồn tại");
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == productID);
            if (existsProduct == null)
                shoppingCart.Add(new CartItem()
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Photo = product.Photo,
                    Quantity = quantity,
                    SalePrice = product.Price,
                });
            else if(existsProduct.Quantity > 1 ||  quantity > 0) 
                existsProduct.Quantity+=quantity;
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Index");
        }
        public IActionResult RemoteFromCart(int productID)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == productID);
            if (index >= 0)
            {
                shoppingCart.RemoveAt(index);
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Index");
        }
    }
}
