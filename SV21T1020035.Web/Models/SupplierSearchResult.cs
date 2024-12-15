using SV21T1020035.DomainModels;

namespace SV21T1020035.Web.Models
{
	public class SupplierSearchResult: PaginationSearchResult
	{
		public required List<Supplier> Data { get; set; }
	}
}
