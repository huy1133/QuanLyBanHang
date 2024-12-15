using Dapper;
using SV21T1020035.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020035.DataLayers.SQLServer
{
	public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>
	{
		public SupplierDAL(string connectionString) : base(connectionString)
		{

		}

		public int Add(Supplier item)
		{
			int result = 0;
			using(var connection = OpenConnection())
			{
				var sql = @"if exists(select * from Suppliers where Email = @Email)
								select -1
							else
								begin
										insert into Suppliers(SupplierName, ContactName, Province, Address, Phone, Email)
										values(@SupplierName, @ContactName, @Province, @Address, @Phone, @Email)
										select SCOPE_IDENTITY();
								end"
                ;
                var parameter = new
				{
                    SupplierName = item.SupplierName ?? "",
                    ContactName = item.ContactName ?? "",
                    Province = item.Province ?? "",
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? ""
                };
				result = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) ;
				connection.Close();
			}
			return result;
		}

		public int Count(string searchValue = "")
		{
			int count = 0;
			searchValue = $"%{searchValue}%";
			using (var connection = OpenConnection())
			{
				var sql = @"select COUNT(*)
							from Suppliers
							where (SupplierName like @searchValue) or (ContactName like @searchValue)";
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
				var sql = @"delete from Suppliers where SupplierID = @SupplierID";
				var parameters = new
				{
                    SupplierID = id,
                };
				result = connection.Execute(sql: sql, param: parameters,commandType: System.Data.CommandType.Text)>0;
				connection.Close ();
			}
			return result;
		}

		public Supplier? Get(int id)
		{
			Supplier? data = null;
			using (var connection = OpenConnection())
			{
				var sql = @"select * from Suppliers where SupplierID = @SupplierID";
				var parameters = new
				{
					SupplierID = id
                };
				data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
			}
			return data;
		}

		public bool InUsed(int id)
		{
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Products where SupplierID = @SupplierID)
	                            select 1
                            else 
	                            select 0";
                var parameters = new
                {
                    SupplierID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

		public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
		{
			List<Supplier> data = new List<Supplier>();
			searchValue = $"%{searchValue}%";
			using (var connection = OpenConnection())
			{
				var sql = @"select * 
							from (
								select * , ROW_NUMBER() over (order by SupplierName) as RowNumber
								from Suppliers
								where (SupplierName like @searchValue) or (ContactName like @searchValue)
							)as t
							where (@pageSize=0) or (t.RowNumber between (@page-1)*@pageSize +1 and @page*@pageSize)
							order by RowNumber";
				var parameters = new
				{
					page, pageSize, searchValue
				};
				data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
				connection.Close();
			}
			return data;
		}

		public bool Update(Supplier item)
		{
			bool result = false;

			using (var connection = OpenConnection())
			{
                var sql = @"
						if exists(select * from Suppliers where SupplierID <> @SupplierID and Email = @Email)
							select -1
						else
							begin							
								update Suppliers
								set SupplierName= @SupplierName, 
									ContactName = @ContactName, 
									Province = @Province, 
									Address = @Address, 
									Phone = @Phone, 
									Email = @Email
									where SupplierID = @SupplierID
							end"
                ;
                var parameters = new
                {
                    SupplierName = item.SupplierName ?? "",
                    ContactName = item.ContactName ?? "",
                    Province = item.Province ?? "",
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    SupplierID = item.SupplierID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}
			return result;	
		}
	}
}
