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
        // Concurrent dictionaries are not enitrely safe. They cannot protect us when we loop through a collection that is being modified.
        private static readonly object _monitor = new object();

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

            // We broadcast to one or more clients depending on the norification type.
            // If we detect that the session is lost we remove it from the dictionary.
            // Must be careful with multithreading and when looping through the gloabal collection.
            HashSet<string> sessionsToRemove = new HashSet<string>();
            List<OperationContext> sessions = new List<OperationContext>();

            // Lock to loop and createa local copy.
            lock(_monitor)
            {
                sessions = _sessionsById.Values.ToList();
            }

            foreach (var ss in sessions)
            {
                var cb = ss.GetCallbackChannel<IVolatilityCallback>();
                switch (n)
                {
                    case Notification.UnexpectedError:
                        if (cb == callback)
                        {
                            // Notify only the caller for its errors.
                            if (!TrySendNotification(cb,n))
                            {
                                sessionsToRemove.Add(ss.SessionId);
                            }
                        }
                        break;
                    default:
                        if (cb != callback)
                        {
                            // Broadcast any data changes to the other clients.
                            if (!TrySendNotification(cb, n))
                            {
                                sessionsToRemove.Add(ss.SessionId);
                            }
                        }

                        break;
                }
            }
            
            // Lock to remove lost sessions.
            if (sessionsToRemove.Count > 0)
            {
                lock(_monitor)
                {
                    foreach (var id in sessionsToRemove)
                    {
                        OperationContext o;
                        _sessionsById.TryRemove(id, out o);
                    }
                }
            }
        }

        // If there the session is lost then remove it from the sessions.
        private static bool TrySendNotification(IVolatilityCallback callback,Notification n)
        {
            try
            {
                callback.SendNotification(n);
                return true;
            }
            catch(Exception ex)
            {
                // This is the exception we get when the session is lost.
                return (ex.HResult == -2146233087);
            }
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
