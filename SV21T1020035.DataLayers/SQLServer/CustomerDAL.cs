using System;
using System.Net;
using System.Numerics;
using Dapper;
using SV21T1020035.DomainModels;

namespace SV21T1020035.DataLayers.SQLServer
{
    public class CustomerDAL : BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer item)
        {
            int newID = 0;
            using(var connection = OpenConnection())
            {
                var sql = @"
                        if exists(select * from Customers where Email = @Email)
                            select -1
                        else
                            begin
                                insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                                values(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked)
                                select SCOPE_IDENTITY();
                            end";
                var parameters = new
                {
                    CustomerName = item.CustomerName ?? "",
                    ContactName = item.ContactName ?? "",
                    Province = item.Province ?? "",
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    IsLocked = item.IsLocked 
                };
                newID = connection.ExecuteScalar<int>(sql: sql,param: parameters,commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return newID;
            //luu y thuc thi cau lenh co cac truong hop sau
            //cau len tra ve tap cac dong hoac cot: su dung lenh Query<T>
            //cau lenh tra ve 1 cot 1 dong: su dung lenh ExecuteScalar<type>
            //cau lenh khong tra ve du lieu ma chi bao so dong tac dong du lieu: Execute
            //cau lenh tra ve 1 dong nhieu cot su dung: QueryFirstOrDefault
            //
            //
            //
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using(var connection = OpenConnection())
            {
                var sql = @"select COUNT(*)
                            from Customers 
                            where (CustomerName like @searchValue) or (ContactName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close() ;    
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Customers where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID=id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? customer = null;
            using(var connection = OpenConnection())
            {
                var sql = @"select * from Customers Where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID = id
                };
                customer = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return customer;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using( var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Orders where CustomerID = @CustomerID)
	                            select 1
                            else 
	                            select 0";
                var parameters = new
                {
                    CustomerID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            searchValue = $"%{searchValue}%";

            using (var connection = OpenConnection())
            {
                var sql = @"select * 
                            from (
	                            select * , ROW_NUMBER() over (order by CustomerName) as RowNumber
	                            from Customers 
	                            where (CustomerName like @searchValue) or (ContactName like @searchValue)
                            )as t
                            where (@pageSize=0) or (t.RowNumber between (@page-1)*@pageSize +1 and @page*@pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Customer>(sql: sql,param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Customer item)
        {
            bool result =false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                        if not exists(select * from Customers where CustomerID <> @CustomerID and Email = @Email)
                            begin
                                update Customers 
                                set CustomerName= @CustomerName, 
	                                ContactName = @ContactName, 
	                                Province = @Province, 
	                                Address = @Address, 
	                                Phone = @Phone, 
	                                Email = @Email, 
	                                IsLocked = @IsLocked
                                where CustomerID = @CustomerID
                            end";
                var parameters = new
                {
                    CustomerName = item.CustomerName ?? "",
                    ContactName = item.ContactName ?? "",
                    Province = item.Province ?? "",
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    IsLocked = item.IsLocked,
                    CustomerID = item.CustomerID 
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
