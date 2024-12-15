using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020035.BusinessLayers;
using SV21T1020035.DomainModels;
using SV21T1020035.Web.Models;

namespace SV21T1020035.Web.Controllers
{
    [Authorize(Roles = $"{UserRoles.ADMIN},{UserRoles.EMPLOYEER}")]
    public class ProductController : Controller
	{
		private const int PAGE_SIDE = 30;
		private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondiotion";
		public IActionResult Index()
		{
            ViewBag.Title = "Quản lý mặt hàng";
			ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
			if (condition == null)
			{
				condition = new ProductSearchInput
				{
					Page = 1,
					PageSize = PAGE_SIDE,
					SearchValue = "",
					CategoryID = 0,
					SupplierID = 0,
					MaxPrice = 0,
					MinPrice = 0,
				};
			}
            return View(condition);
		}
		public IActionResult Search(ProductSearchInput condition)
		{
			int rowCount = 0;
			var data = ProductDataService.ListProducts(
				out rowCount,
				condition.Page,
				condition.PageSize,
				condition.SearchValue,
				condition.CategoryID,
				condition.SupplierID,
				condition.MinPrice,
				condition.MaxPrice
			);
			ProductSearchResult model = new ProductSearchResult()
			{
				Page = condition.Page,
				PageSize = condition.PageSize,
				SearchValue = condition.SearchValue,
				CategoryID = condition.CategoryID,
				SupplierID = condition.SupplierID,
				MaxPrice = condition.MaxPrice,
				MinPrice = condition.MinPrice,
				RowCount = rowCount,
				Data = data
			};
			ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, model);
			return View(model);
		}
		public IActionResult Create()
		{
			ViewBag.Title = "Thêm sản phẩm";
			var data = new Product
			{
				ProductID = 0,
				IsSelling = true,
			};
			return View("Edit",data);
		}
		public IActionResult Edit(int id)
		{
			ViewBag.Title = "Sữa thông tin sản phẩm";
			Product? data = ProductDataService.GetProduct(id);
			if (data == null)
			{
				return RedirectToAction("Index");
			}
			return View(data);
		}
		public IActionResult Delete(int id)
		{
			ViewBag.Title = "Xóa sản phẩm";
			var data = ProductDataService.GetProduct(id);
			if (Request.Method == "POST")
			{
				bool result = ProductDataService.DeleteProduct(id);
				return RedirectToAction("Index");
			}
			return View(data);
		}
		[HttpPost]
		public IActionResult Save(Product data, IFormFile? uploadPhoto)
		{
			if (String.IsNullOrWhiteSpace(data.ProductName)) 
				ModelState.AddModelError(nameof(data.ProductName), "Vui lòng nhập tên mặt hàng");
            if (String.IsNullOrWhiteSpace(data.ProductDescription))
                ModelState.AddModelError(nameof(data.ProductDescription), "Vui lòng nhập mô tả sản phẩm");
            if (data.CategoryID==0)
                ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
            if (data.SupplierID==0)
                ModelState.AddModelError(nameof(data.SupplierID), "vui lòng chọn nhà cung cấp");
            if (String.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Vui lòng nhập đơn vị tính");
			if(data.Price<=0)
			{
                ModelState.AddModelError(nameof(data.Price), $"Vui lòng nhập lại giá hàng");
            }
			if (!ModelState.IsValid)
			{
				return View("Edit",data);
			}

            String filePath = "";
			if (uploadPhoto != null)
			{
				string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
				filePath = Path.Combine(ApplicationContext.WebRootPath,"images","products", fileName);
				data.Photo = fileName;
			}
			if (data.ProductID == 0)
			{
                int result = ProductDataService.AddProduct(data);
				if (result > 0)
				{
					if(uploadPhoto != null)
					{
						using(var stream = new FileStream(filePath, FileMode.Create))
						{
							uploadPhoto.CopyTo(stream);
						}
					}
                    return RedirectToAction("Edit", new { id = result });
                }
			}
			else
			{
				bool result = ProductDataService.UpdateProduct(data);
				if (result)
				{
                    if (uploadPhoto != null)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            uploadPhoto.CopyTo(stream);
                        }
                    }
                }
			}
            return RedirectToAction("Edit", data);
        }
		public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
		{
			switch (method)
			{
				case "add":
					ViewBag.Title = "Bổ xung ảnh cho mặt hàng";
					var data = new ProductPhoto
					{
						ProductID = id,
						IsHidden = false,
						PhotoID = photoId
					};
					return View(data);
				case "edit":
                    ViewBag.Title = "Thay đổi ảnh cho mặt hàng";
					data = ProductDataService.GetProductPhoto(photoId);
                    if (data == null)
                    {
                        return RedirectToAction("Edit", new { id = id });
                    }
                    return View(data);
				case "delete":
					bool result = ProductDataService.DeleteProductPhoto(photoId);
					return RedirectToAction("Edit", new { id = id });
				default:
					return RedirectToAction("Index");
            }
		}
		[HttpPost]
		public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
		{
			if(uploadPhoto == null&&data.Photo==null)
			{
				ModelState.AddModelError(nameof(data.Photo), "Vui lòng thêm ảnh");
			}
			if (string.IsNullOrWhiteSpace(data.Description))
			{
				ModelState.AddModelError(nameof(data.Description), "Vui lòng nhập mô tả cho ảnh");
			}
            if(data.DisplayOrder<=0)
            {
                ModelState.AddModelError(nameof(data.DisplayOrder), $"Vui lòng nhập lại thứ tự hiển thị");
            }
			if (!ModelState.IsValid)
			{
				return View("Photo",data);
			}
            String filePath = "";
			if(uploadPhoto != null)
			{
				String fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
				filePath = Path.Combine(ApplicationContext.WebRootPath,"images","products",fileName);
				data.Photo = fileName;
			}
			if (data.PhotoID == 0)
			{
				long result = ProductDataService.AddProductPhoto(data);
				if (result > 0)
				{
					if (uploadPhoto != null)
					{
						using(var stream = new FileStream(filePath, FileMode.Create))
						{
							uploadPhoto.CopyTo(stream);
						}
					}
				}
				else
				{
					ModelState.AddModelError(nameof(data.DisplayOrder),"Thứ tự hiển thị đã được sử dụng cho bức ảnh khác");
                    return View("Photo", data);
                }
			}
			else
			{
				bool result = ProductDataService.UpdateProductPhoto(data);
				if (result)
				{
                    if (uploadPhoto != null)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            uploadPhoto.CopyTo(stream);
                        }
                    }
				}
				else
				{
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã được sử dụng cho bức ảnh khác");
                    return View("Photo", data);
                }
			}
			return RedirectToAction("Edit", new {id = data.ProductID});
		}
		public IActionResult Attribute(int id=0, string method="",int attributeId = 0)
		{
			switch (method)
			{
                case "add":
                    ViewBag.Title = "Bổ xung thuộc tính cho mặt hàng";
					var data = new ProductAttribute
					{
						ProductID = id,
						AttributeID = attributeId
					};
                    return View(data);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính mặt hàng";
					data = ProductDataService.GetProductAttribute(attributeId);
					if(data == null)
					{
                        return RedirectToAction("Edit", new { id = id });
                    }
                    return View(data);
                case "delete":
					bool result = ProductDataService.DeleteProductAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
		}
		[HttpPost]
		public IActionResult SaveAttribute(ProductAttribute data)
		{
			if (string.IsNullOrWhiteSpace(data.AttributeName))
				ModelState.AddModelError(nameof(data.AttributeName), "Vui lòng nhập tên thuộc tính");
			if (string.IsNullOrWhiteSpace(data.AttributeValue))
				ModelState.AddModelError(nameof(data.AttributeValue), "Vui lòng nhập giá trị cho thuộc tính");
			if (data.DisplayOrder <= 0)
				ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng lại dữ liệu tự hiển thị");
			if (!ModelState.IsValid)
			{
				return View("Attribute", data);
			}
			if(data.AttributeID == 0)
			{
				long result = ProductDataService.AddProductAttribute(data);
				if(result == 0)
				{
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã được sử dụng cho thuộc tính khác khác");
                    return View("Attribute", data);
                }
			}
			else
			{
				bool result = ProductDataService.UpdateProductAttribuite(data);
				if(!result)
				{
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã được sử dụng cho thuộc tính khác khác");
                    return View("Attribute", data);
                }
			}
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
    }
}
