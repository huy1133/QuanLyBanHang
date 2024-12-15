using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DomainModels
{
	/// <summary>
	/// Mặt hàng 
	/// </summary>
	public class Category
	{
		public int CategoryID { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}
}
