using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Web.Models;
using System.Runtime.CompilerServices;

namespace SV21T1020035.Web.Controllers
{
    [Authorize(Roles = $"{UserRoles.ADMIN},{UserRoles.EMPLOYEER}")]
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 30;
        private const string SUPPLIER_SEARCH_CONDITION = "SupplierSearchCondition";
        public IActionResult Index()
        {
            ViewBag.Title = "Quản lý nhà cung cấp";
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH_CONDITION);
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
            var data = CommomDataService.ListOfSupplier(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            SupplierSearchResult model = new SupplierSearchResult()
            {
                Page = condition.Page,  
                PageSize = condition.PageSize,
                RowCount = rowCount,
                SearchValue = condition.SearchValue??"",
                Data = data
            };
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Sửa thông tin nhà cung cấp";
            var data = CommomDataService.GetSupplier(id);
            return View(data);
        }
        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                bool result = CommomDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            var data = CommomDataService.GetSupplier(id);
            ViewBag.Title = "Xóa nhà cung cấp";
            return View(data);
        }
        public IActionResult Create()
        {
            var data = new Supplier()
            {
                SupplierID = 0,
            };
            ViewBag.Title = "Thêm nhà cung cấp";
            return View("Edit",data);
        }
        [HttpPost]
        public IActionResult Save(Supplier data)
        {
            if (string.IsNullOrWhiteSpace(data.SupplierName))
            {
                ModelState.AddModelError(nameof(data.SupplierName), "Vui lòng nhập tên nhà cung cấp");
            }
            if (string.IsNullOrWhiteSpace(data.ContactName))
            {
                ModelState.AddModelError(nameof(data.ContactName), "Vui lòng nhập tên liên hệ");
            }
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            }
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                ModelState.AddModelError(nameof(data.Email), "Vui lòng địa chỉ email");
            }
            if (string.IsNullOrWhiteSpace(data.Address))
            {
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ");
            }
            if (string.IsNullOrWhiteSpace(data.Province))
            {
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");
            }
            if(!ModelState.IsValid)
            {
                return View("Edit", data);
            }
            if (data.SupplierID == 0)
            {
                var result = CommomDataService.AddSupplier(data);
                if (result <= 0)
                {
                    ModelState.AddModelError(nameof(data.Email), "Địa chỉ email đã được sử dụng");
                    return View("Edit", data);
                }
            }
            else
            {
                var result = CommomDataService.UpdateSupplier(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Email), "Địa chỉ email này đã được sử dụng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
