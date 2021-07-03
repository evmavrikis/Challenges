using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Collections.Concurrent;
using VolatilityContracts;
using Newtonsoft.Json;
using System.Configuration;

namespace VolatilityWCFService
{
    class SessionManager
    {
        private static ConcurrentDictionary<string, OperationContext> _sessionsById = new ConcurrentDictionary<string, OperationContext>();
        private static ConcurrentDictionary<int, CustomerDetails> _customerDetailsById = new ConcurrentDictionary<int, CustomerDetails>();
        static void Main(string[] args)
        {
            LoadCustomerRecords();
            var host = new ServiceHost(typeof(VolatilityService), new Uri[] { new Uri("net.pipe://localhost") });
            host.AddServiceEndpoint(typeof(IVolatilityService), new NetNamedPipeBinding(), "MyAddress");
            
            host.Open();

            Console.WriteLine("Service is available. " +
        "Press <ENTER> to exit.");
            Console.ReadLine();
        }

        internal static void AddSession(OperationContext context)
        {
            _sessionsById.TryAdd(context.SessionId, context);
        }

        internal static void SendNotification(object state)
        {
            object[] ar = state as object[];
            var callback = (IVolatilityCallback)ar[0];
            var n = (Notification)ar[1];
            callback.SendNotification(n);
        }
        internal static void LoadCustomerRecords()
        {
            var fileName = ConfigurationManager.AppSettings["DataFile"];
            var json = System.IO.File.ReadAllText(fileName);
            var records = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerDetails>>(json);
            records.ForEach(r => _customerDetailsById.TryAdd(r.Id, r));
        }

        internal static ConcurrentDictionary<int, CustomerDetails> CustomerDetailsById
        {
            get
            {
                return _customerDetailsById;
            }
        }
    }
}
