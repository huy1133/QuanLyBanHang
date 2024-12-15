using Dapper;
using SV21T1020035.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DataLayers.SQLServer
{
	public class EmployeerDAL : BaseDAL, ICommonDAL<Employeer>
	{
		public EmployeerDAL(string connectionString) : base(connectionString)
		{
		}

		public int Add(Employeer item)
		{
			int result = 0;
			using(var connection = OpenConnection())
			{
				var sql = @"
                            if exists (select * from Employees where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Employees(FullName,BirthDate, Address, Phone, Email, Password, Photo, IsWorking)
							        values(@FullName,@BirthDate, @Address, @Phone, @Email, @Password, @Photo, @IsWorking)
							        select SCOPE_IDENTITY();
                                end";
				var parameters = new
				{
                    FullName = item.FullName ?? "",
                    BirthDate = item.BirthDate,
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    Password = item.Password ?? "",
                    Photo = item.Photo ?? "",
                    IsWorking = item.IsWorking
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
			using(var connection = OpenConnection())
			{
				var sql = @"select count(*)
					from Employees
					where (FullName like @searchValue)";
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
                var sql = @"delete from Employees where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

		public Employeer? Get(int id)
		{
            Employeer? employeer = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID,FullName,BirthDate, Address, Phone, Email,Password,Photo,IsWorking
                            from Employees where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                employeer = connection.QueryFirstOrDefault<Employeer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return employeer;
        }

		public bool InUsed(int id)
		{
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Orders where EmployeeID = @EmployeeID)
	                            select 1
                            else 
	                            select 0";
                var parameters = new
                {
                    EmployeeID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

		public List<Employeer> List(int page = 1, int pageSize = 0, string searchValue = "")
		{
			List<Employeer> data = new List<Employeer>();
			searchValue = $"%{searchValue}%";
			using(var connection = OpenConnection())
			{
				var sql = @"select EmployeeID,FullName,BirthDate, Address, Phone, Email,Password,Photo, IsWorking,RowNumber
					from (
						select * , ROW_NUMBER() over (order by FullName) as RowNumber
						from Employees
						where (FullName like @searchValue)
					) as t
					where t.RowNumber between (@page-1)*@pageSize + 1 and @page *@pageSize";
				var parameters = new
				{
					page, pageSize, searchValue
				};
				data = connection.Query<Employeer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
				connection.Close();
			}
			return data;
		}

		public bool Update(Employeer item)
		{
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                             if not exists (select * from Employees where EmployeeID <> @EmployeeID and Email = @Email)
                                begin
                                    update Employees
							        set FullName = @FullName,
                                        BirthDate = @BirthDate,
                                        Address = @Address,
                                        Phone = @Phone,
                                        Email = @Email,
                                        Password = @Password,
                                        Photo = @Photo,
                                        IsWorking = @IsWorking
							        where EmployeeID = @EmployeeID
                                end";
                var parameters = new
                {
                    FullName = item.FullName ?? "",
                    BirthDate = item.BirthDate,
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    Password = item.Password ?? "",
                    Photo = item.Photo ?? "",
                    IsWorking = item.IsWorking,
                    EmployeeID = item.EmployeeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
	}
}
