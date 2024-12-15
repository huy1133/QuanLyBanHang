using SV21T1020035.DomainModels;

namespace SV21T1020035.Web.Models
{
    public class OrderDetailModel
    {
        public required Order Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
        public decimal TotalPrice
        {
            get
            {
                decimal total = 0;
                if(Details != null)
                {
                    foreach (var item in Details)
                    {
                        total += item.TotalPrice;
                    }
                }
                return total;   
            }
        }
    }
}
