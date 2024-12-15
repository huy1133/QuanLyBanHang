using Dapper;
using SV21T1020035.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DataLayers.SQLServer
{
	public class CategoryDAL : BaseDAL, ICommonDAL<Category>
	{
		public CategoryDAL(string connectionString) : base(connectionString)
		{
		}

		public int Add(Category item)
		{
			int result = 0;
			using(var connection = OpenConnection())
			{
				var sql = @"insert into Categories(CategoryName,Description)
							values(@CategoryName,@Description)
							 select SCOPE_IDENTITY();";
				var parameters = new
				{
                    CategoryName = item.CategoryName ?? "",
                    Description = item.Description ?? "",
                };
				result = connection.ExecuteScalar<int>(sql, parameters,commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return result;
		}

		public int Count(string searchValue = "")
		{
			int count = 0;
			searchValue = $"%{searchValue}%";
			using(var connection = OpenConnection())
			{
				var sql = @"select COUNT(*)
							from Categories
							where (CategoryName like @searchValue) or (Description like @searchValue)";
				var parameters = new
				{
					searchValue
				};
				count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return count;
		}

		public bool Delete(int id)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"delete from Categories where CategoryID = @CategoryID";
				var parameters = new
				{
                    CategoryID = id
                };
				result = connection.Execute(sql,parameters,commandType: System.Data.CommandType.Text)>0;
				connection.Close();
			}
			return result;
		}

		public Category? Get(int id)
		{
			Category? result = null;
			using (var connection = OpenConnection()) 
			{
				var sql = @"select * from Categories where CategoryID = @CategoryID";
				var parameters = new
				{
					CategoryID = id
                };
				result = connection.QueryFirstOrDefault<Category>(sql, parameters, commandType: System.Data.CommandType.Text);
				connection.Close ();
			}
			return result;
		}

		public bool InUsed(int id)
		{
			bool result = false;
			using(var connection = OpenConnection())
			{
				var sql = @"if exists (select * from Products where CategoryID = @CategoryID)
	                            select 1
                            else 
	                            select 0";
				var parameters = new
				{
                    CategoryID = id
                };
				result = connection.ExecuteScalar<bool>(sql,parameters,commandType: System.Data.CommandType.Text);
				connection.Close() ;
            }
			return result;
		}

		public List<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
		{
			List<Category> data = new List<Category>();
			searchValue = $"%{searchValue}%";
			using (var connection = OpenConnection())
			{
				var sql = @"select * 
							from (
								select * , ROW_NUMBER() over (order by CategoryName) as RowNumber
								from Categories
								where (CategoryName like @searchValue) or (Description like @searchValue)
							)as t
							where (@pageSize=0) or (t.RowNumber between (@page-1)*@pageSize +1 and @page*@pageSize)
							order by RowNumber";
				var parameters = new
				{
					page, pageSize, searchValue
				};
				data = connection.Query<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
				connection.Close();
			}
			return data;
		}

		public bool Update(Category item)
		{
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Categories
							set CategoryName = @CategoryName,
								Description = @Description
							where CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryName = item.CategoryName ??"",
                    Description = item.Description ??"",
                    CategoryID = item.CategoryID
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result;
        }
	}
}
