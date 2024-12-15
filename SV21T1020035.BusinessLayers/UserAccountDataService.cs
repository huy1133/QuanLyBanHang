using SV21T1020035.DataLayers;
using SV21T1020035.DataLayers.SQLServer;
using SV21T1020035.DomainModels;

namespace SV21T1020035.BusinessLayers
{
    public class UserAccountDataService
    {
        private static readonly IUserAccountDAL UserAccountDB;

        static UserAccountDataService()
        {
            UserAccountDB = new UserAccountDAL(Configuration.ConnectionString);
        }

        public static UserAccount? Authorize(TypeAccount account,String userName, String password)
        {
            switch (account)
            {
                case TypeAccount.Employeer:
                    return UserAccountDB.Authorize(userName, password);
                case TypeAccount.Customer:
                    return null;
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
                    return true;
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
                    return true;
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
