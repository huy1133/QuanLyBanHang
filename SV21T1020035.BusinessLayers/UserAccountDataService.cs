using SV21T1020035.DataLayers;
using SV21T1020035.DataLayers.SQLServer;
using SV21T1020035.DomainModels;

namespace SV21T1020035.BusinessLayers
{
    public class UserAccountDataService
    {
        private static readonly IUserAccountDAL<UserAccount> UserAccountDB;
        private static readonly IUserAccountDAL<CustomerAccount> CustomerAccountDB;
        static UserAccountDataService()
        {
            string connectionString
             = Configuration.ConnectionString;
            UserAccountDB = new UserAccountDAL(connectionString);
            CustomerAccountDB = new CustomerAccountDAL(connectionString);
        }

        public static dynamic? Authorize(TypeAccount account,String userName, String password)
        {
            switch (account)
            {
                case TypeAccount.Employeer:
                    return UserAccountDB.Authorize(userName, password);
                case TypeAccount.Customer:
                    return CustomerAccountDB.Authorize(userName, password);
                default:
                    return null;
            }
        }
        public static bool ChangePassword(TypeAccount account,String userName,String password)
        {
            switch (account)
            {
                case TypeAccount.Employeer:
                    return UserAccountDB.ChangePassword(userName, password);
                case TypeAccount.Customer:
                    return CustomerAccountDB.ChangePassword(userName, password);
                default:
                    return false;
            }
        }
        public static bool VerifyOldPassword(TypeAccount account,String userName,String password)
        {
            switch (account)
            {
                case TypeAccount.Employeer:
                    return UserAccountDB.VerifyOldPassword(userName, password);
                case TypeAccount.Customer:
                    return CustomerAccountDB.VerifyOldPassword(userName, password);
                default:
                    return false;
            }
        }
        public enum TypeAccount
        {
            Employeer,
            Customer
        }
    }
}
