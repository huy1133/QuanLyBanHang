using Dapper;
using SV21T1020035.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DataLayers.SQLServer
{
    public class UserAccountDAL : BaseDAL, IUserAccountDAL
    {
        public UserAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string userName, string password)
        {
            UserAccount? userAccount = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID as UserId, Email as UserName, FullName as DisplayName, Photo,  RoleName 
                            from Employees 
                            where Email = @UserName and Password = @Password";
                var parameters = new
                {
                    Username = userName,
                    Password = password
                };
                userAccount = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            };
            return userAccount;
        }

        public bool ChangePassword(string userName, string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Employees
                            set Password = @Password
                            where Email = @UserName";
                var paramerters = new {
                    Password = password,
                    UserName = userName
                };
                result = connection.Execute(sql: sql, param: paramerters, commandType: System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result;
        }

        public bool VerifyOldPassword(string userName, string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Employees where Email = @UserName and Password = @Password)
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
