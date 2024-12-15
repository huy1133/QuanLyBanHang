using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DomainModels
{
    public class UserAccount
    {
        public String UserId { get; set; } = ""; 
        public String UserName { get; set; } = "";
        public String DisplayName { get; set; } = "";
        public String Photo { get; set; } = "";
        public String RoleName { get; set; } = "";
    }
}
