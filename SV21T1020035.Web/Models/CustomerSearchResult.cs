using SV21T1020035.DomainModels;

namespace SV21T1020035.Web.Models
{
    public class CustomerSearchResult: PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
}
