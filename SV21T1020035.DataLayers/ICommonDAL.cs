using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DataLayers
{
    /// <summary>
    /// dinh nghia cac phep xu ly chung
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// tìm kiến và lấy danh sách dưới dạng phân trang
        /// </summary>
        /// <param name="page">trang vần hiển thị</param>
        /// <param name="pageSize">số dòng dwx liệu cần hiển thị trên mỗi trang (bằng 0 nếu không phân trang)</param>
        /// <param name="searchValue">giá trị dữ liệu cần tìm (rỗng nếu muốn lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        List<T> List(int page = 1, int pageSize = 0, string searchValue = "");
        /// <summary>
        /// dem so dong du lieu tim duoc
        /// </summary>
        /// <param name="searchValue">gia tri can tim</param>
        /// <returns></returns>
        int Count(string searchValue = "");
        /// <summary>
        /// bo xung du lieu vao CSDL, ham tra ve ID cua du lieu duoc bo xung
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int Add(T item);
        /// <summary>
        /// cap nhat du lieu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Update(T item);
        /// <summary>
        /// xoa du lieu dua vao id
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// lay 1 dong du lieu dua vao id (tra ve null neu du lieu khong ton tai)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);
        /// <summary>
        /// kiem tra 1 dong du lieu co khoa id hien co du lieu lien quan den bang khac hay khong
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}
