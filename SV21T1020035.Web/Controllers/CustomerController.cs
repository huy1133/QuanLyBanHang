using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Web.Models;

namespace SV21T1020035.Web.Controllers
{
    [Authorize(Roles = $"{UserRoles.ADMIN},{UserRoles.EMPLOYEER}")]
    public class CustomerController : Controller
	{
		private const int PAGE_SIZE = 30;
		private const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";
		public IActionResult Index()
		{
            ViewBag.Title = "Quản lý khách hàng";
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);
			if (condition == null)
			{
				condition = new PaginationSearchInput()
				{
					Page = 1,
					PageSize = PAGE_SIZE,
					SearchValue = ""
				};
			}
			return View(condition);
		}
		public IActionResult Search(PaginationSearchInput condition)
		{
			int rowCount;
			var data = CommomDataService.ListOfCustomer(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
			CustomerSearchResult model = new CustomerSearchResult()
			{
				Page = condition.Page,
				PageSize = condition.PageSize,
				SearchValue = condition.SearchValue??"",
				RowCount = rowCount,
				Data = data
			};
			ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, condition);
			return View(model);
		}
		public IActionResult Create()
		{
			ViewBag.Title = "Thêm khách hàng";
			var data = new Customer()
			{
				CustomerID = 0,
				IsLocked = false,
			};
			return View("Edit",data);
		}
		public IActionResult Edit(int id = 0)
		{
			ViewBag.Title = "Sửa thông tin khách hàng";
			var data = CommomDataService.GetCustomer(id);
			if(data == null)
			{
				return RedirectToAction();
			}
			return View(data);
		}
        [HttpPost]
		public IActionResult Save(Customer data)
		{
			// kiem tra cac du lieu dau vao co hop le
			// neu kiem tra du lieu khong hop le thi luu tru thong bao loi vaf tron ModelState
			if (string.IsNullOrWhiteSpace(data.CustomerName))
			{
				ModelState.AddModelError(nameof(data.CustomerName), "Vui lòng nhập tên khách hàng");//luu tru thong bao loi de chuyen cho view
			}
            if (string.IsNullOrWhiteSpace(data.ContactName))
            {
                ModelState.AddModelError(nameof(data.ContactName), "Vui lòng nhập tên giao dịch");//luu tru thong bao loi de chuyen cho view
            }
            if (string.IsNullOrWhiteSpace(data.Address))
            {
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ");//luu tru thong bao loi de chuyen cho view
            }
            if (string.IsNullOrWhiteSpace(data.Province))
            {
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");//luu tru thong bao loi de chuyen cho view
            }
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");//luu tru thong bao loi de chuyen cho view
            }
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập địa chỉ email");//luu tru thong bao loi de chuyen cho view
            }
			//TODO: kiem tra du lieu dau vao 
			if (!ModelState.IsValid)
			{
				return View("Edit",data);
			}
            if (data.CustomerID == 0)
			{
				int id = CommomDataService.AddCustomer(data);
				if(id <= 0)
				{
                    ModelState.AddModelError(nameof(data.Email), "Địa chỉ email đã được sử dụng");
                    return View("Edit", data);
                }
			}
			else
			{
				bool result = CommomDataService.UpdateCustomer(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Email), "Địa chỉ email đã được sử dụng");
                    return View("Edit", data);
                }
            }
			return RedirectToAction("Index");
		}
        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa Khách Hàng";
            if (Request.Method == "POST")
            {
				bool result = CommomDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var data = CommomDataService.GetCustomer(id);
            if (data == null)
            {
                return RedirectToAction("Index");

            }
            return View(data);
        }
    }
}
