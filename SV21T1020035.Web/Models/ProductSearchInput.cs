namespace SV21T1020035.Web.Models
{
    public class ProductSearchInput: PaginationSearchInput
    {
        /// <summary>
        /// Mã loại hàng 
        /// </summary>
        public int CategoryID { get; set; } = 0;
        /// <summary>
        /// Mã nhà cung cấp
        /// </summary>
        public int SupplierID { get; set; } = 0;
        /// <summary>
        /// Giá từ
        /// </summary>
        public decimal MinPrice { get; set; } = 0;
        /// <summary>
        /// Giá đến 
        /// </summary>
        public decimal MaxPrice { get; set; } = 0;
    }
}
