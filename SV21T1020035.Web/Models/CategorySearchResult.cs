using SV21T1020035.DomainModels;

namespace SV21T1020035.Web.Models
{
	public class CategorySearchResult: PaginationSearchResult
	{
		public required List<Category> Data { get; set; }
	}
}
