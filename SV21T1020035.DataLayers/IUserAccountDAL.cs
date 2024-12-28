namespace SV21T1020035.DataLayers
{
    /// <summary>
    /// định nghĩa các phép xử lý dữ liệu liên quan đến người dùng 
    /// </summary>
    public interface IUserAccountDAL<T> where T : class
    {
        /// <summary>
        /// Xác thực tài khoản đăng nhập của người dùng 
        /// Hàm trả về thông tin tài khoản nếu xát thực thành công 
        /// Ngược lại trả về null 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        T? Authorize(String userName, String password);
        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ChangePassword(String userName, String password);
        /// <summary>
        /// Kiểm tra mật khẩu củ đã đúng hay chưa
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool VerifyOldPassword(String userName, String password);
    }
}
