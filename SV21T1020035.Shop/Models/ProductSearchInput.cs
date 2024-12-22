namespace SV21T1020035.Shop.Models
{
    public class ProductSearchInput
    {
        /// <summary>
        /// Mã loại hàng
        /// </summary>
        public int CategoryID { get; set; } = 0;
        /// <summary>
        /// Chuỗi chứa giá trị cần tìm kiếm 
        /// </summary>
        public string SearchValue { get; set; } = "";
        /// <summary>
        /// trang
        /// </summary>
        public int Page { get; set; } = 1;
    }
}
