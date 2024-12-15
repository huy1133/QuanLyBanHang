using Azure;
using Dapper;
using Microsoft.Data.SqlClient;
using SV21T1020035.DomainModels;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DataLayers.SQLServer
{
	public class ProductDAL : BaseDAL, IProductDAL
	{
		public ProductDAL(string connectionString) : base(connectionString)
		{
		}
		public List<Product> List(int page = 1, int pageSize = 0,string searchValue = "",
							   int categoryID = 0, 
							   int supplierID = 0,
							   decimal minPrice = 0, decimal maxPrice = 0)
		{
			List<Product> data = new List<Product>();
			searchValue = $"%{searchValue}%";
			using( var connection = OpenConnection())
			{
				var sql = @"SELECT *
							FROM (
							SELECT *,
							ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber
							FROM Products
							WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
							AND (@CategoryID = 0 OR CategoryID = @CategoryID)
							AND (@SupplierID = 0 OR SupplierID = @SupplierID)
							AND (Price >= @MinPrice)
							AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
							) AS t
							WHERE (@PageSize = 0)
							OR (RowNumber BETWEEN (@Page - 1)*@PageSize + 1 AND @Page * @PageSize)";
				var parameters = new {
					SearchValue = searchValue,
					CategoryID = categoryID,
					SupplierID = supplierID,
					MinPrice = minPrice,
					MaxPrice = maxPrice,
					PageSize = pageSize,
					Page = page
				};
				data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
				connection.Close();
			}

			return data;
		}
		public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0,
					  decimal minPrice = 0, decimal maxPrice = 0)
		{
			int count = 0;
			searchValue = $"%{searchValue}%";
			using( var connection = OpenConnection())
			{
				var sql = @"
						SELECT count(*)
						FROM (
						SELECT *,
						ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber
						FROM Products
						WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
						AND (@CategoryID = 0 OR CategoryID = @CategoryID)
						AND (@SupplierID = 0 OR SupplierID = @SupplierID)
						AND (Price >= @MinPrice)
						AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
						) AS t
						";
				var parameters = new
				{
					SearchValue = searchValue,
					CategoryID = categoryID,
					SupplierID = supplierID,
					MinPrice = minPrice,
					MaxPrice = maxPrice
				};
				count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return count;
		}
		public Product? Get(int productID)
		{
			Product? result = null;
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @"SELECT * FROM Products WHERE ProductID = @ProductID";
				var parameters = new { ProductID = productID };
				result= connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return result;
		}
		public int Add(Product data)
		{
			int result = 0;
			using (var connection = OpenConnection())
			{
				var sql = @"insert into Products(ProductName,ProductDescription,SupplierID,CategoryID,Unit,Price,Photo,IsSelling)
						values(@ProductName,@ProductDescription,@SupplierID,@CategoryID,@Unit,@Price,@Photo,@IsSelling)
						select SCOPE_IDENTITY();";
				var parameters = new
				{
					ProductName = data.ProductName??"",
					ProductDescription= data.ProductDescription ?? "",
					SupplierID = data.SupplierID,
					CategoryID = data.CategoryID,
					Unit = data.Unit ?? "",
					Price = data.Price,
					Photo = data.Photo,
					IsSelling = data.IsSelling
				};
				result = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return result;
		}
		public bool Update(Product data)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"update Products 
						set ProductName = @ProductName,
							ProductDescription = @ProductDescription,
							SupplierID = @SupplierID,
							CategoryID = @CategoryID,
							Unit = @Unit,
							Price = @Price,
							Photo = @Photo,
							IsSelling = @IsSelling
						WHERE ProductID = @ProductID";
				var parameters = new
				{
					ProductName = data.ProductName ?? "",
					ProductDescription = data.ProductDescription ?? "",
					SupplierID = data.SupplierID,
					CategoryID = data.CategoryID,
					Unit = data.Unit ?? "",
					Price = data.Price,
					Photo = data.Photo,
					IsSelling = data.IsSelling,
					ProductID = data.ProductID
				};
				result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)>0;
				connection.Close();
			}
			return result;
		}
		public bool Delete(int productID)
		{
			bool result = false;

			using (var connection = OpenConnection())
			{
				var sql = @"delete from ProductPhotos where ProductID = @ProductID
							delete from ProductAttributes where ProductID = @ProductID 
							delete from Products where ProductID = @ProductID";
				var parameters = new
				{
					ProductID = productID
				};
				result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}
			return result;
		}
		public bool InUsed(int productID)
		{
			bool result = false;
			using(var connection = OpenConnection())
			{
				var sql = @"if exists (select * from OrderDetails where ProductID = @ProductID)
	                            select 1
                            else 
	                            select 0";
				var parameters = new
				{
					ProductID = productID
				};
				result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close ();
			}
			return result;	
		}
		public List<ProductPhoto> ListPhotos(int productID)
		{
			List<ProductPhoto> data = new List<ProductPhoto>();
			using(var connection = OpenConnection())
			{
				var sql = @"select * 
						from ProductPhotos
						where ProductID = @ProductID 
						order by DisplayOrder";
				var parameters = new
				{
					ProductID = productID
				};
				data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
				connection.Close();
			}
			return data;
		}
		public ProductPhoto? GetPhoto(long photoID)
		{
			ProductPhoto? data = null;
			using(var connection = OpenConnection())
			{
				var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
				var parameters = new {
					PhotoID = photoID
				};
				data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return data;
		}
		public long AddPhoto(ProductPhoto data)
		{
			long result = 0;
			using( var connection = OpenConnection())
			{
				var sql = @"if exists (select * from ProductPhotos where DisplayOrder = @DisplayOrder and ProductID = @ProductID)
								select 0
							else
								begin
									insert into ProductPhotos(ProductID,Photo,Description,DisplayOrder,IsHidden)
									values(@ProductID,@Photo,@Description,@DisplayOrder,@IsHidden)
									select SCOPE_IDENTITY();
								end";
				var parameters = new
				{
					ProductID= data.ProductID,
					Photo =data.Photo ?? "",
					Description = data.Description ?? "",
					DisplayOrder = data.DisplayOrder,
					IsHidden = data.IsHidden
				};
				result = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return result;
		}
		public bool UpdatePhoto(ProductPhoto data)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if not exists(select * from ProductPhotos where ProductID = @ProductID and DisplayOrder = @DisplayOrder and PhotoID <> @PhotoID)
							begin 
								UPDATE ProductPhotos
									SET Photo = @Photo,
										Description = @Description,
										DisplayOrder = @DisplayOrder,
										IsHidden = @IsHidden
									WHERE ProductID = @ProductID AND PhotoID = @PhotoID
							end";

				var parameters = new
				{
					PhotoID = data.PhotoID,
					ProductID = data.ProductID,
					Photo = data.Photo ?? "",
					Description = data.Description ?? "",
					DisplayOrder = data.DisplayOrder,
					IsHidden = data.IsHidden
				};

				int rowsAffected = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				result = rowsAffected > 0;
				connection.Close();
			}
			return result;
		}
		public bool DeletePhoto(long photoID)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"DELETE FROM ProductPhotos WHERE PhotoID = @PhotoID";

				var parameters = new
				{
					PhotoID = photoID
				};

				int rowsAffected = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				result = rowsAffected > 0;
				connection.Close();
			}
			return result;
		}
		public List<ProductAttribute> ListAttributes(int productID)
		{
			List<ProductAttribute> data = new List<ProductAttribute>();
			using (var connection = OpenConnection())
			{
				var sql = @"SELECT * FROM ProductAttributes 
                    WHERE ProductID = @ProductID 
                    ORDER BY DisplayOrder";

				var parameters = new {
					ProductID = productID 
				};
				data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
				connection.Close();
			}
			return data;
		}
		public ProductAttribute? GetAttribute(long attributeID)
		{
			ProductAttribute? result = null;
			using (var connection = OpenConnection())
			{
				var sql = @"SELECT * FROM ProductAttributes WHERE AttributeID = @AttributeID";
				var parameters = new { 
					AttributeID = attributeID 
				};
				result = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return result;
		}
		public long AddAttribute(ProductAttribute data)
		{
			long result = 0;
			using (var connection = OpenConnection())
			{
				var sql = @"if exists (select * from ProductAttributes where DisplayOrder = @DisplayOrder and ProductID = @ProductID)
								select 0
							else
								begin
									INSERT INTO ProductAttributes (ProductID, AttributeName, AttributeValue, DisplayOrder) 
									VALUES (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
									SELECT SCOPE_IDENTITY();
								end";

				var parameters = new
				{
					ProductID = data.ProductID,
					AttributeName = data.AttributeName ?? "",
					AttributeValue = data.AttributeValue ?? "",
					DisplayOrder = data.DisplayOrder
				};

				result = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return result;
		}
		public bool UpdateAttribute(ProductAttribute data)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if not exists(select * from ProductAttributes where ProductID = @ProductID and DisplayOrder = @DisplayOrder and AttributeID <> @AttributeID)
							begin 
								UPDATE ProductAttributes 
									SET AttributeName = @AttributeName,
										AttributeValue = @AttributeValue,
										DisplayOrder = @DisplayOrder
									WHERE AttributeID = @AttributeID AND ProductID = @ProductID
							end";

				var parameters = new
				{
					AttributeID = data.AttributeID,
					ProductID = data.ProductID,
					AttributeName = data.AttributeName ?? "",
					AttributeValue = data.AttributeValue ?? "",
					DisplayOrder = data.DisplayOrder
				};

				int rowsAffected = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				result = rowsAffected > 0;
				connection.Close();
			}
			return result;
		}
		public bool DeleteAttribute(long attributeID)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"DELETE FROM ProductAttributes WHERE AttributeID = @AttributeID";

				var parameters = new { AttributeID = attributeID };
				int rowsAffected = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				result = rowsAffected > 0;
				connection.Close();
			}
			return result;
		}

	}
}
