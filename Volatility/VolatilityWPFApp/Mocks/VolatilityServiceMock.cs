using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatilityContracts;
using System.Configuration;
using VolatilityWPFApp.Extensions;

namespace VolatilityWPFApp.Mocks
{
    /// <summary>
    /// Mock implementation vor the service.
    /// </summary>
    internal class VolatilityServiceMock:IVolatilityService
    {
        // Do not use a dictionary to simulate some delay.
        private List<CustomerDetails> _customers;
        private IVolatilityCallback _callback;
        public VolatilityServiceMock(IVolatilityCallback callback)
        {
            var fileName = ConfigurationManager.AppSettings["MockData"];
            var json = System.IO.File.ReadAllText(fileName);
            _customers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerDetails>>(json);
            _callback = callback;
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
            var cust = _customers.FirstOrDefault(c => c.Id == Id);
            if (cust == null)
            {
                return false;
            }
            else
            {
                _customers.Remove(cust);
                _callback.SendNotification(Notification.RecordDeleted);
                return true;
            }
        }
        public bool UpdateCustomer(CustomerDetails customerDetails)
        {
            var cust = _customers.FirstOrDefault(c => c.Id == customerDetails.Id);
            if (cust == null)
            {
                return false;
            }
            else
            {
                _customers.Remove(cust);
                var newCust = new CustomerDetails();
                newCust.CopyFrom(customerDetails);
                _customers.Add(newCust);
                _callback.SendNotification(Notification.RecordUpdated);
                return true;
            }
            
        }
        public bool AddNewCustomer(CustomerDetails customerDetails)
        {
            var maxId = _customers.Max(c => c.Id);
            customerDetails.Id = maxId + 1;
            _customers.Add(customerDetails);
            _callback.SendNotification(Notification.RecordAdded);
            return true;
        }
    }
}
