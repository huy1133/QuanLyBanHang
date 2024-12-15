using Dapper;
using SV21T1020035.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DataLayers.SQLServer
{
	public class ShipperDAL : BaseDAL, ICommonDAL<Shipper>
	{
		public ShipperDAL(string connectionString) : base(connectionString)
		{
		}

		public int Add(Shipper item)
		{
			int result = 0;
			using(var connection = OpenConnection())
			{
				var sql = @"insert into Shippers(ShipperName,Phone)
							values(@ShipperName,@Phone)
							select SCOPE_IDENTITY();";
				var parameters = new
				{
					ShipperName = item.ShipperName ?? "",
					Phone = item.Phone ?? "",
				};
				result = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return result;
		}

		public int Count(string searchValue = "")
		{
			int count = 0;
			searchValue = $"%{searchValue}%";
			using(var connecion = OpenConnection())
			{
				var sql = @"select COUNT(*)
							from Shippers
							where (ShipperName like @searchValue)";
				var parameters = new
				{
					searchValue
				};
				count = connecion.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connecion.Close();
			}
			return count;
		}

		public bool Delete(int id)
		{
			bool result = false;
			using(var connection = OpenConnection())
			{
				var sql = @"delete from Shippers where ShipperID = @ShipperID";
				var parameters = new
				{
                    ShipperID = id,
                };
				result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)>0;
				connection.Close();
			}
			return result;

		}

		public Shipper? Get(int id)
		{
			Shipper? result = null;
			using (var connecion = OpenConnection())
			{
				var sql = @"select * from Shippers
							where ShipperID = @ShipperID";
				var parameters = new
				{
                    ShipperID = id
                };
				result = connecion.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connecion.Close();
			}
			return result;
		}

		public bool InUsed(int id)
		{
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Orders where ShipperID = @ShipperID)
	                            select 1
                            else 
	                            select 0";
                var parameters = new
                {
                    ShipperID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

		public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
		{
			List<Shipper> data = new List<Shipper>();
			searchValue = $"%{searchValue}%";
			using (var connecion = OpenConnection())
			{
				var sql = @"select * 
							from (
								select * , ROW_NUMBER() over (order by ShipperName) as RowNumber
								from Shippers
								where (ShipperName like @searchValue) 
							)as t
							where (@pageSize=0) or (t.RowNumber between (@page-1)*@pageSize +1 and @page*@pageSize)
							order by RowNumber";
				var parameters = new
				{
					page, pageSize, searchValue
				};
				data = connecion.Query<Shipper>(sql: sql, param: parameters,commandType: System.Data.CommandType.Text).ToList();
				connecion.Close();
			}
			return data;
		}

		public bool Update(Shipper item)
		{
			bool result = false;
			using(var connecion = OpenConnection())
			{
				var sql = @"update Shippers
							set ShipperName = @ShipperName,
								Phone = @Phone
							where ShipperID = @ShipperID";
				var parameters = new
				{
                    ShipperName = item.ShipperName ?? "",
                    Phone = item.Phone ?? "",
                    ShipperID = item.ShipperID

                };
				result = connecion.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)>0;
				connecion.Close();
			}
			return result;
		}
	}
}
