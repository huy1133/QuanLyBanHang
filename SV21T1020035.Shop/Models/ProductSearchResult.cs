using SV21T1020035.DomainModels;

namespace SV21T1020035.Shop.Models
{
    public class ProductSearchResult
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string SearchValue { get; set; } = "";

        public int RowCount { get; set; }

        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                {
                    return 1;
                }
                int c = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                {
                    c += 1;
                }
                return c;
            }
        }
        public int CategoryID { get; set; } = 0;

        public required List<Product> Data { get; set; }
    }
}
