using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Web.Models;

namespace SV21T1020035.Web.Controllers
{
    [Authorize(Roles = $"{UserRoles.ADMIN},{UserRoles.EMPLOYEER}")]
    public class ShipperController : Controller
	{
		private const int PAGE_SIZE = 30;
		private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";
		public IActionResult Index()
		{
            ViewBag.Title = "Quản lý giao hàng";
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
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
			var data = CommomDataService.ListOfShipper(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
			ShipperSearchResult model = new ShipperSearchResult()
			{
				Page = condition.Page,
				PageSize = condition.PageSize,
				RowCount = rowCount,
				SearchValue = condition.SearchValue ?? "",
				Data = data
			};
			ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);
			return View(model);
		}
		public IActionResult Edit(int id)
		{
			var data = CommomDataService.GetShipper(id);
			ViewBag.Title = "Sửa thông tin giao hàng";
			return View(data);
		}
        public IActionResult Create()
        {
			var data = new Shipper()
			{
				ShipperID = 0
			};
            ViewBag.Title = "Thêm giao hàng";
            return View("Edit",data);
        }
        public IActionResult Delete(int id)
        {
            var data = CommomDataService.GetShipper(id);
            if (Request.Method == "POST")
			{
				bool result = CommomDataService.DeleteShipper(id);
				return RedirectToAction("Index");
			}
            ViewBag.Title = "Xóa giao hàng";
            return View(data);
        }
		[HttpPost]
		public IActionResult Save(Shipper data)
		{
			if (string.IsNullOrWhiteSpace(data.ShipperName))
			{
				ModelState.AddModelError(nameof(data.ShipperName), "Vui lòng nhập tên giao hàng");
			}
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại giao hàng");
            }
			if (!ModelState.IsValid)
			{
				return View("Edit", data);
			}
            if (data.ShipperID == 0)
			{
				int result = CommomDataService.AddShipper(data);
			}
			else
			{
				bool result = CommomDataService.UpdateShipper(data);
			}
			return RedirectToAction("Index");
		}
    }
}
