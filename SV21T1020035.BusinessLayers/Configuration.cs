using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.BusinessLayers
{
	public class Configuration
	{
		private static string connectionString = "";
		/// <summary>
		/// khởi tạo cấu hình cho businessLayer
		/// </summary>
		/// <param name="connectionString"></param>
		public static void Initialize(string connectionString)
		{
			Configuration.connectionString = connectionString;
		}
		/// <summary>
		/// tham số kết nối cơ sở dữ liệu
		/// </summary>
		public static string ConnectionString
		{
			get { return connectionString; }
		}
	}
}
