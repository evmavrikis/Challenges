using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatilityContracts;

namespace VolatilityWPFApp
{
    internal class VolatilityService:IVolatilityService
    {
        public IEnumerable<Customer> GetCustomers()
        {
            return null;
        }
        public IEnumerable<Customer> GetCustomers(RequestFilters filters)
        {
            return null;
        }
        public CustomerDetails GetCustomerDetails(int Id)
        {
            return null;
        }
        public bool DeleteCustomer(int Id)
        {
            return true;
        }
        public bool UpdateCustomer(Customer customer)
        {
            return true;
        }
        public bool AddNewCustomer(Customer customer)
        {
            return false;
        }
    }
}
