using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DomainModels
{
    /// <summary>
    /// Trạng thái đơn hàng 
    /// </summary>
    public class Constants
    {
        public const int ORDER_INIT = 1;
        public const int ORDER_ACCEPTED = 2;
        public const int ORDER_SHIPPING = 3;
        public const int ORDER_FINISHED = 4;
        public const int ORDER_CANCEL = -1;
        public const int ORDER_REJECTED = -2;
    }

    public class UserRoles
    {
        public const String ADMIN = "admin";
        public const String EMPLOYEER = "employeer";
    }
}
