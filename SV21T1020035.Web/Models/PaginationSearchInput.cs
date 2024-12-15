namespace SV21T1020035.Web.Models
{
    public class PaginationSearchInput
    {
        /// <summary>
        /// trang cần hiển thị
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// số dòng hiển thị trên mỗi trang 
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// chuỗi chứa giá trị cần tim kiếm
        /// </summary>
        public string SearchValue { get; set; } = "";
    }
}
