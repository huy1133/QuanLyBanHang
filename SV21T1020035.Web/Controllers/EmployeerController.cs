using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Web.Models;
using System.Globalization;

namespace SV21T1020035.Web.Controllers
{
	[Authorize(Roles = $"{UserRoles.ADMIN}")]
    public class EmployeerController : Controller
	{
		private const int PAGE_SIZE = 30;
		private const string EMPLOYEER_SEARCH_CONDITION = "EmployeerSearchCondition";
		public IActionResult Index()
		{
            ViewBag.Title = "Quản lý nhân viên";
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEER_SEARCH_CONDITION);
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
			var data = CommomDataService.ListOfEmployeer(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
			EmployeerSearchResult model = new EmployeerSearchResult()
			{
				Page = condition.Page,
				PageSize = condition.PageSize,
				RowCount = rowCount,
                SearchValue = condition.SearchValue ?? "",
                Data = data
			};
			ApplicationContext.SetSessionData(EMPLOYEER_SEARCH_CONDITION, condition);
			return View(model);
		}
		public IActionResult Create()
		{
			ViewBag.Title = "Thêm nhân viên";
			var data = new Employeer()
			{
				EmployeeID = 0,
				IsWorking = true,
				Photo = "",
				BirthDate = DateTime.Now
			};
            return View("Edit",data);
		}
		public IActionResult Edit(int id = 0)
		{
			ViewBag.Title = "Sửa thông tin nhân viên";
			var data = CommomDataService.GetEmployeer(id);
			if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
		public IActionResult Delete(int id=0)
		{
			if (Request.Method == "POST")
			{
				CommomDataService.DeleteEmployeer(id);
                return RedirectToAction("Index");
            }
            var data = CommomDataService.GetEmployeer(id);
            ViewBag.Title = "Xóa nhân viên";
            return View(data);
		}
		[HttpPost]
		public IActionResult Save(Employeer employeer, string _birthDate, IFormFile? uploadPhoto)
		{	
			if(string.IsNullOrWhiteSpace(employeer.FullName)){
				ModelState.AddModelError(nameof(employeer.FullName), "Vui lòng nhập tên nhân viên");
			}
            if (string.IsNullOrWhiteSpace(employeer.Email))
            {
                ModelState.AddModelError(nameof(employeer.Email), "Vui lòng nhập địa chỉ email");
            }
            if (string.IsNullOrWhiteSpace(employeer.Address))
            {
                ModelState.AddModelError(nameof(employeer.Address), "Vui lòng nhập địa chỉ");
            }
            if (string.IsNullOrWhiteSpace(employeer.Phone))
            {
                ModelState.AddModelError(nameof(employeer.Phone), "Vui lòng nhập số điện thoại");
            }
            DateTime? date = _birthDate.ToDateTime();
			if (date != null)
			{
				employeer.BirthDate = date.Value;
				if(date.Value.Year <= 1753)
				{
					ModelState.AddModelError(nameof(employeer.BirthDate), "Ngày sinh không hợp lệ");
				}
			}
			else 
			{ 
				ModelState.AddModelError(nameof(employeer.BirthDate), "Vui không để trống ngày sinh"); 
			}
			if (!ModelState.IsValid) 
			{ 
				return View("Edit",employeer);
			}
			string filename ="";
			string filePath ="";
            if (uploadPhoto != null)
            {
				filename = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
				filePath = Path.Combine(ApplicationContext.WebRootPath, "images", "Employee", filename);
                employeer.Photo = filename;
            }
            if (employeer.EmployeeID == 0)
			{
				int result = CommomDataService.AddEmployeer(employeer);
				if (result <= 0)
				{
					ModelState.AddModelError(nameof(employeer.Email), "Địa chỉ email này đã được sử dụng");
					return View("Edit", employeer);
				}
                else if(uploadPhoto != null)

                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                }
			}
			else
			{
				bool result = CommomDataService.UpdateEmployeer(employeer);
                if (!result)
                {
                    ModelState.AddModelError(nameof(employeer.Email), "Địa chỉ email này đã được sử dụng");
                    return View("Edit", employeer);
				}
				else if (uploadPhoto != null)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                }
            }
            return RedirectToAction("Index");
        }
		

	}
}
