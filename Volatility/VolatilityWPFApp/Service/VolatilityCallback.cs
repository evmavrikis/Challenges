using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatilityContracts;

namespace VolatilityWPFApp
{
    internal class VolatilityCallback : IVolatilityCallback
    {

        private Action<Notification> _internalCallback;
        public VolatilityCallback(Action<Notification> del)
        {
            _internalCallback = del;
        }
        public void SendNotification(Notification notification)
        {
            _internalCallback(notification);
        }
    }
   
}
