using SV21T1020035.DomainModels;

namespace SV21T1020035.Web.Models
{
    public class OrderSearchResult: PaginationSearchResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public required List<Order> Data { get; set; } = new List<Order>();
    }
}
