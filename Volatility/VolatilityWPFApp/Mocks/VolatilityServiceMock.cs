using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatilityContracts;
using System.Configuration;

namespace VolatilityWPFApp.Mocks
{
    /// <summary>
    /// Mock implementation vor the service.
    /// </summary>
    internal class VolatilityServiceMock:IVolatilityService
    {
        
        private List<CustomerDetails> _customers;

        public VolatilityServiceMock()
        {
            var fileName = ConfigurationManager.AppSettings["MockData"];
            var json = System.IO.File.ReadAllText(fileName);
            _customers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerDetails>>(json);
        }
        public IEnumerable<Customer> GetCustomers()
        {
            return _customers;
        }
        public IEnumerable<Customer> GetCustomers(RequestFilters filters)
        {
            var fn = filters.FirstName;
            var ln = filters.LastName;

            var ret = _customers.Where(c => (fn == "" || String.Compare(c.FirstName.Substring(0, fn.Length), fn, true) == 0 && 
                (ln == "" || String.Compare(c.LastName.Substring(0, ln.Length), ln, true) == 0)));
            return ret;
        }
        public CustomerDetails GetCustomerDetails(int Id)
        {
            var ret = _customers.FirstOrDefault(c => c.Id == Id);
            return ret;
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
