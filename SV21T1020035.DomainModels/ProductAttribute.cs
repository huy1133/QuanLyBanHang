using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DomainModels
{
	/// <summary>
	/// Thuộc tính sản phẩm 
	/// </summary>
	public class ProductAttribute
	{
		public long AttributeID { get; set; }
		public int ProductID { get; set; }
		public string AttributeName { get; set; } = "";
		public string AttributeValue { get; set; } = "";
		public int DisplayOrder { get; set; }
	}
}
