using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatilityContracts;
using System.Timers;
using System.Threading;

namespace VolatilityWPFApp.Mocks
{
    internal class VolatiltiyCallbackMock:IVolatilityCallback
    {
        private System.Timers.Timer _timer;
        private Random _rnd = new Random();
        private Action<Notification> _del;

        public VolatiltiyCallbackMock(Action<Notification> act)
        {
            _timer = new System.Timers.Timer(2000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _del = act;
        }
        public void SendNotification(Notification notification)
        {
            ThreadPool.QueueUserWorkItem(SendNotification2, notification);
        }

        private  void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (_rnd.NextDouble() < 0.35)
            {
                var v = _rnd.Next(0, 3);
                switch (v)
                {
                    case 0:
                        SendNotification(Notification.RecordUpdated);
                        break;
                    case 1:
                        SendNotification(Notification.RecordAdded);
                        break;
                    case 2:
                        SendNotification(Notification.RecordDeleted);
                        break;
                }
                
            }
        }

        private void SendNotification2(object state)
        {
            var n = (Notification)state;
            _del(n);
        }
    }
}
