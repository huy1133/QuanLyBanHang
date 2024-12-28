using SV21T1020035.DataLayers;
using SV21T1020035.DataLayers.SQLServer;
using SV21T1020035.DomainModels;

namespace SV21T1020035.BusinessLayers
{
    public static class CommomDataService
    {
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Employeer> employeerDB;
        private static readonly ISimpleSelectDAL<Province> provinceDB;
        /// <summary>
        /// ctor
        /// </summary>
        static CommomDataService()
        {
            string connectionString
            = Configuration.ConnectionString;
            customerDB = new CustomerDAL(connectionString);
			supplierDB = new SupplierDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            employeerDB = new EmployeerDAL(connectionString);
            provinceDB = new ProvinceDAL(connectionString);
		}
        //customer
        public static List<Customer> ListOfCustomer(out int rowCount, int page = 1,int pageSize = 10, string searchValue = "" )
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);
        }
        public static List<Customer> ListOfCustomer()
        {
            return customerDB.List(1,0, "");
        }
        public static int AddCustomer(Customer customer)
        {
            return customerDB.Add(customer);
        }
        public static bool DeleteCustomer(int id)
        {
            return customerDB.Delete(id);
        }
        public static bool UpdateCustomer(Customer customer)
        {
            return customerDB.Update(customer);
        }
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        public static bool InUseCustomer(int id)
        {
            return customerDB.InUsed(id);
        }
        //supplier
        public static List<Supplier> ListOfSupplier(out int rowCount, int page = 1, int pageSize = 10, string searchValue = "")
        {
			rowCount = supplierDB.Count(searchValue);
			return supplierDB.List(page, pageSize, searchValue);
		}
        public static List<Supplier> ListOfSupplier(string searchValue = "")
        {
            return supplierDB.List(1, 0, searchValue);
        }
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        public static bool DeleteSupplier(int id)
        {
            return supplierDB.Delete(id);
        }
        public static bool UpdateSupplier(Supplier supplier)
        {
            return supplierDB.Update(supplier);
        }
        public static int AddSupplier(Supplier supplier)
        {
            return supplierDB.Add(supplier);
        }
        public static bool InUseSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }
        //category
        public static List<Category> ListOfCategory(out int rowCount, int page = 1, int pageSize = 10, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize,searchValue);
        }
        public static List<Category> ListOfCategory(string searchValue = "")
        {
            return categoryDB.List(1, 0, searchValue);
        }
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        public static bool DeleteCategory(int id)
        {
            return categoryDB.Delete(id);
        }
        public static bool UpdateCategory(Category category)
        {
            return categoryDB.Update(category);
        }
        public static int AddCategory(Category category)
        {
            return categoryDB.Add(category);
        }
        public static bool InUserCategory(int id)
        {
            return categoryDB.InUsed(id);
        }
        //shipper
        public static List<Shipper> ListOfShipper(out int rowCount, int page = 1, int pageSize = 10, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize,searchValue);
        }
        public static List<Shipper> ListOfShipper()
        {
            return shipperDB.List(1, 0, "");
        }
        public static bool UpdateShipper(Shipper shipper)
        {
            return shipperDB.Update(shipper);
        }
        public static bool DeleteShipper(int id)
        {
            return shipperDB.Delete(id);
        }
        public static int AddShipper(Shipper shipper)
        {
            return shipperDB.Add(shipper);
        }
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        public static bool InUseShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        //emlpoyee
        public static List<Employeer> ListOfEmployeer(out int rowCount, int page = 1, int pageSize = 10, string searchValue = "")
        {
            rowCount = employeerDB.Count(searchValue);
            return employeerDB.List(page,pageSize,searchValue);
        }
        public static int AddEmployeer(Employeer employeer)
        {
            return employeerDB.Add(employeer);
        }
        public static bool DeleteEmployeer(int id)
        {
            return employeerDB.Delete(id);
        }
        public static bool UpdateEmployeer(Employeer employeer)
        {
            return employeerDB.Update(employeer);
        }
        public static bool InUseEmployeer(int employeerId)
        {
            return employeerDB.InUsed(employeerId);
        }
        public static Employeer? GetEmployeer(int id)
        {
            return employeerDB.Get(id);
        }

        //province
        public static List<Province> ListOfProvince()
        {
            return provinceDB.List();
        }
    }
}
