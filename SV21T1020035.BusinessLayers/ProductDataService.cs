using SV21T1020035.DataLayers.SQLServer;
using SV21T1020035.DomainModels;


namespace SV21T1020035.BusinessLayers
{
	public static class ProductDataService
	{
		private static readonly ProductDAL productDB;
		/// <summary>
		/// ctor
		/// </summary>
		static ProductDataService()
		{
			productDB = new ProductDAL(Configuration.ConnectionString);
		}
		public  static List<Product> ListProducts(out int rowCount ,int page = 1, int pageSize = 0, string searchValue = "",
							   int categoryID = 0,
							   int supplierID = 0,
							   decimal minPrice = 0, decimal maxPrice = 0)
		{
			rowCount = productDB.Count(searchValue, categoryID, supplierID, minPrice, maxPrice);
			return productDB.List(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice);
		}
		public static List<Product> ListProducts (string searchValue ="")
		{
			return productDB.List(1,0,searchValue,0,0,0,0);
		}
		public static Product? GetProduct(int productID)
		{
			return productDB.Get(productID);
		}
		public static int AddProduct(Product data)
		{
			return productDB.Add(data);
		}
		public static bool DeleteProduct(int productID)
		{
			return productDB.Delete(productID);
		}
		public static bool UpdateProduct(Product data)
		{
			return productDB.Update(data);
		}
		public static bool InUsedProduct(int productID) 
		{
			return productDB.InUsed(productID);
		}
		public static List<ProductPhoto> ListOfProductPhotos(int productID)
		{
			return productDB.ListPhotos(productID);
		}
		public static ProductPhoto? GetProductPhoto(int photoID) 
		{
			return productDB.GetPhoto(photoID);
		}
		public static long AddProductPhoto(ProductPhoto data)
		{
			return productDB.AddPhoto(data);
		}
		public static bool UpdateProductPhoto(ProductPhoto data)
		{
			return productDB.UpdatePhoto(data);
		}
		public static bool DeleteProductPhoto(int photoID)
		{
			return productDB.DeletePhoto(photoID);
		}
		public static List<ProductAttribute> ListOfProductAttributes(int productID)
		{
			return productDB.ListAttributes(productID);
		}
		public static ProductAttribute? GetProductAttribute(int attributeID)
		{
			return productDB.GetAttribute(attributeID);
		}
		public static long AddProductAttribute(ProductAttribute data)
		{
			return productDB.AddAttribute(data);
		}
		public static bool UpdateProductAttribuite(ProductAttribute data)
		{
			return productDB.UpdateAttribute(data);
		}
		public static bool DeleteProductAttribute(int attributeID)
		{
			return productDB.DeleteAttribute(attributeID);
		}
	}
}
