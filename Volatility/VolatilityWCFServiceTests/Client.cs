using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatilityContracts;
using System.ServiceModel;
using System.Collections.Concurrent;

namespace VolatilityWCFServiceTests
{
    public class Client:IVolatilityCallback
    {
        private readonly object _monitor = new object();
        
        internal IVolatilityService Service;
        internal  InstanceContext Context;
        internal ConcurrentDictionary<Notification, long> Notifications = new ConcurrentDictionary<Notification, long>();

        public Client()
        {
            var vals = Enum.GetValues(typeof(Notification)).Cast<Notification>();
            foreach (var v in vals)
            {
                Notifications.TryAdd(v, 0);
            }

        }
        internal bool InitWCFConnection()
        {
            var address = new EndpointAddress("net.pipe://localhost/MyAddress");
            var binding = new NetNamedPipeBinding()
            {
                MaxBufferSize = 2000000,
                MaxReceivedMessageSize = 2000000,
                CloseTimeout = new TimeSpan(0,0,10)
            };

            Context = new InstanceContext(this);

            var factory = new DuplexChannelFactory<IVolatilityService>(Context, binding, address);
            Service = factory.CreateChannel();
            return Service.Ping();
        }

        public void SendNotification(Notification notification)
        {
            lock (_monitor)
            {
                Notifications[notification]++;
            }
        }

        public void TryClose()
        {
            try
            {
                Context.Close();
            }
            catch
            {

            }
        }
    }
}
