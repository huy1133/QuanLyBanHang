using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Web.Models;

namespace SV21T1020035.Web.Controllers
{
	[Authorize(Roles = $"{UserRoles.ADMIN},{UserRoles.EMPLOYEER}")]
	public class CategoryController : Controller
	{
		private const int PAGE_SIZE = 30;
		private const string Category_Search_Condition = "categorySearchCondition";
		public IActionResult Index()
		{
            ViewBag.Title = "Quản lý loại hàng";
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(Category_Search_Condition);
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
			var data = CommomDataService.ListOfCategory(out rowCount, condition.Page, condition.PageSize,condition.SearchValue?? "");
			CategorySearchResult model = new CategorySearchResult()
			{
				Page = condition.Page,
				PageSize = condition.PageSize,
				SearchValue = condition.SearchValue??"",
				RowCount = rowCount,
				Data = data
			};
			ApplicationContext.SetSessionData(Category_Search_Condition, condition);
			return View(model);

		}
		public IActionResult Create()
		{
			Category data = new Category()
			{
				CategoryID = 0
			};
			ViewBag.Title = "Thêm loại hàng";
			return View("Edit",data);
		}
        public IActionResult Edit(int id)
        {
			var data = CommomDataService.GetCategory(id);
            ViewBag.Title = "Sửa thông tin loại hàng";
            return View(data);
        }
        public IActionResult Delete(int id)
        {
			var data = CommomDataService.GetCategory(id);
			if(Request.Method == "POST")
			{
				bool result = CommomDataService.DeleteCategory(id);
				return RedirectToAction("Index");
			}
            ViewBag.Title = "Xóa loại hàng";
            return View(data);
        }
		[HttpPost]
		public IActionResult Save(Category data)
		{
			if (string.IsNullOrWhiteSpace(data.CategoryName))
			{
				ModelState.AddModelError(nameof(data.CategoryName), "Vui lòng nhập tên loại hàng");
			}
            if (string.IsNullOrWhiteSpace(data.Description))
            {
                ModelState.AddModelError(nameof(data.Description), "Vui lòng nhập mô tả");
            }
			if (!ModelState.IsValid) { return View("Edit", data); }
            if (data.CategoryID==0)
			{
				int result = CommomDataService.AddCategory(data);
			}
			else
			{ 
				bool result = CommomDataService.UpdateCategory(data);
			}
			return RedirectToAction("Index");
		}
    }
}
