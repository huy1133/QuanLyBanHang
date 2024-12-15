using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DataLayers
{
    public interface ISimpleSelectDAL<T> where T : class
    {
        /// <summary>
        /// Select toàn bộ dữ liệu 
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
