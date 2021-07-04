using System;
using System.Collections.Generic;
using System.Linq;
using VolatilityContracts;
using System.Configuration;
using VolatilityWPFApp.Extensions;
using System.Timers;
using System.Threading.Tasks;

namespace VolatilityWPFApp.Mocks
{
    /// <summary>
    /// Mock implementation vor the service. it can fire notifications to the callback using a different thread.
    /// It loads the customer records from a data json file.
    /// </summary>
    internal class VolatilityServiceMock:IVolatilityService
    {
        // Do not use a dictionary to simulate some delay.
        private List<CustomerDetails> _customers;
        private IVolatilityCallback _callback;

        private System.Timers.Timer _timer;
        private Random _rnd = new Random();
        
        public VolatilityServiceMock(IVolatilityCallback callback)
        {
            var fileName = ConfigurationManager.AppSettings["MockData"];
            var json = System.IO.File.ReadAllText(fileName);
            _customers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerDetails>>(json);
            _callback = callback;

            _timer = new System.Timers.Timer(2000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Time handler usded to fire semi-random notification via the callback.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (_rnd.NextDouble() < 0.35)
            {
                var v = _rnd.Next(0, 3);
                switch (v)
                {
                    case 0:
                       
                        Task.Run(()=> _callback.SendNotification(Notification.RecordUpdated));
                        break;
                    case 1:
                        Task.Run(() => _callback.SendNotification(Notification.RecordAdded));
                        break;
                    case 2:
                        Task.Run(() => _callback.SendNotification(Notification.RecordDeleted));
                        break;
                }

            }
        }

        public bool Ping()
        {
            return true;
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
                Task.Run(() => _callback.SendNotification(Notification.RecordDeleted));
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
                Task.Run(() => _callback.SendNotification(Notification.RecordUpdated));
                return true;
            }
            
        }
        public bool AddNewCustomer(CustomerDetails customerDetails)
        {
            var maxId = _customers.Max(c => c.Id);
            customerDetails.Id = maxId + 1;
            _customers.Add(customerDetails);
            Task.Run(() => _callback.SendNotification(Notification.RecordAdded));
            return true;
        }

        
    }
}
