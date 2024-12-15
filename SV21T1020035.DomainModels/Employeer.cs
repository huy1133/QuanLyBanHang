﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020035.DomainModels
{
	/// <summary>
	/// Nhân viên 
	/// </summary>
	public class Employeer
	{
		public int EmployeeID { get; set; }
		public string FullName { get; set; } = string.Empty;
		public DateTime BirthDate { get; set;}
		public string Address { get; set;} = string.Empty;
		public string Phone { get; set;} = string.Empty ;
		public string Email { get; set;} = string.Empty;
		public string Password { get; set;} = string.Empty;
		public string Photo { get; set;} = string.Empty;
		public bool IsWorking { get; set;}
	}
}