using Dapper;
using SV21T1020035.DomainModels;

namespace SV21T1020035.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL<CustomerAccount>
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public CustomerAccount? Authorize(string userName, string password)
        {
            CustomerAccount? customerAccount = null;
            using(var connection = OpenConnection())
            {
                var sql = @"select CustomerID as CustomerId, Email as CustomerName, CustomerName as DisplayName
                            from Customers 
                            where Email = @UserName and Password = @Password";
                var parameter = new
                {
                    Username = userName,
                    Password = password
                };
                customerAccount = connection.QueryFirstOrDefault<CustomerAccount>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return customerAccount;
        }
        public bool ChangePassword(string userName, string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Customers
                            set Password = @Password
                            where Email = @UserName";
                var paramerters = new
                {
                    Password = password,
                    UserName = userName
                };
                result = connection.Execute(sql: sql, param: paramerters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool VerifyOldPassword(string userName, string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Customers where Email = @UserName and Password = @Password)
	                            select 1
                            else 
	                            select 0";
                var paramerters = new
                {
                    Password = password,
                    UserName = userName
                };
                result = connection.ExecuteScalar<int>(sql: sql, param: paramerters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
