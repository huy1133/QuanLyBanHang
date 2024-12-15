namespace SV21T1020035.Web.Models
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Photo { get; set; } = "";
        public string Unit { get; set; } = "";
        public int Quantity { get; set; } 
        public decimal SalePrice { get; set; }
        public decimal TotalPrice { 
            get
            {
                return Quantity * SalePrice;
            }
        }
    }
}
