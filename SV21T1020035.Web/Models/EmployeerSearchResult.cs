using SV21T1020035.DomainModels;

namespace SV21T1020035.Web.Models
{
	public class EmployeerSearchResult: PaginationSearchResult
	{
		public required List<Employeer> Data { get; set; }
	}
}
