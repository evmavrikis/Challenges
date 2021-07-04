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
using System.Timers;

namespace VolatilityWCFService
{
    /// <summary>
    /// Simple session/context manager for the WCF clients.
    /// It caches all the customer records and it updates the physical file using a timer.
    /// </summary>
    class SessionManager
    {
        // Concurrent dictionaries are not enitrely safe. They cannot protect us when we loop through a collection that is being modified.
        private static readonly object _monitor = new object();

        private static ConcurrentDictionary<string, OperationContext> _sessionsById = new ConcurrentDictionary<string, OperationContext>();
        private static ConcurrentDictionary<int, CustomerDetails> _customerDetailsById = new ConcurrentDictionary<int, CustomerDetails>();
        private static System.Timers.Timer _timer = new System.Timers.Timer();
        private static bool _dataFileBusy = false;

        static void Main(string[] args)
        {
            LoadCustomerRecords();

            // Setup timer for persisting the customer changes in the json file.
            int secs;
            if (int.TryParse(ConfigurationManager.AppSettings["SaveDataIntervalSeconds"], out secs))
            {
                _timer = new System.Timers.Timer(secs*1000);
                _timer.Elapsed += SaveCustomers;
                _timer.AutoReset = true;
                _timer.Enabled = true;
            }

            // Setup and start the service:
            var host = new ServiceHost(typeof(VolatilityService), new Uri[] { new Uri(ConfigurationManager.AppSettings["BaseUri"]) });
            host.AddServiceEndpoint(typeof(IVolatilityService), new NetNamedPipeBinding(), ConfigurationManager.AppSettings["EndPoint"]);
            
            host.Open();

            Console.WriteLine("Volatility Service is now running. " +"Press <ENTER> to exit.");
            Console.ReadLine();
        }

        internal static void AddSession(OperationContext context)
        {
            _sessionsById.TryAdd(context.SessionId, context);
        }

        
        internal static void SendNotification(IVolatilityCallback callback, Notification n)
        {
           
            // We broadcast to one or more clients depending on the norification type.
            // If we detect that the session is lost we remove it from the dictionary.
            // Must be careful with multithreading and when looping through the gloabal collection.
            HashSet<string> sessionsToRemove = new HashSet<string>();
            List<OperationContext> sessions = new List<OperationContext>();

            // Lock to loop and createa local copy.
            lock (_monitor)
            {
                sessions = _sessionsById.Values.ToList();
            }

            foreach (var ss in sessions)
            {
                // Remove sessions thet have terminated or faulted gracefully.
                if (ss.InstanceContext.State == CommunicationState.Closed || ss.InstanceContext.State == CommunicationState.Faulted)
                {
                    sessionsToRemove.Add(ss.SessionId);
                    continue;
                }

                var cb = ss.GetCallbackChannel<IVolatilityCallback>();
                switch (n)
                {
                    case Notification.UnexpectedError:
                        if (cb == callback)
                        {
                            // Notify only the caller for its errors.
                            if (!TrySendNotification(cb, n))
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
                lock (_monitor)
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
            _dataFileBusy = true;

            var fileName = ConfigurationManager.AppSettings["DataFile"];
            var json = System.IO.File.ReadAllText(fileName);
            var records = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerDetails>>(json);
            records.ForEach(r => _customerDetailsById.TryAdd(r.Id, r));

            _dataFileBusy = false;
        }

        internal static ConcurrentDictionary<int, CustomerDetails> CustomerDetailsById
        {
            get
            {
                return _customerDetailsById;
            }
        }

        private static void SaveCustomers(Object source, ElapsedEventArgs e)
        {
            if (_dataFileBusy)
            {
                return;
            }

            try
            {
                var fileName = ConfigurationManager.AppSettings["DataFile"];
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(_customerDetailsById.Values);
                System.IO.File.WriteAllText(fileName, json);
            }
            catch
            {
                List<OperationContext> sessions;
                lock (_monitor)
                {
                    sessions = _sessionsById.Values.ToList();
                }
                foreach (var s in sessions)
                {
                    var cb = s.GetCallbackChannel<IVolatilityCallback>();
                    TrySendNotification(cb, Notification.UnexpectedError);
                }
            }
            
        }
    }
}
