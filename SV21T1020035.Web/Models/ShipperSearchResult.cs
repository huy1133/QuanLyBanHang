using SV21T1020035.DomainModels;

namespace SV21T1020035.Web.Models
{
	public class ShipperSearchResult: PaginationSearchResult
	{
		public required List<Shipper> Data { get; set; }
	}
}
