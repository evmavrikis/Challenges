using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using VolatilityContracts;
using VolatilityWCFService.Extensions;
using System.Threading;


namespace VolatilityWCFService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class VolatilityService : IVolatilityService
    {
        private IVolatilityCallback _callBack;
        public VolatilityService()
        {
            SessionManager.AddSession(OperationContext.Current);
            _callBack = OperationContext.Current.GetCallbackChannel<IVolatilityCallback>();
            Console.WriteLine("{0} New session {1}",DateTime.Now ,OperationContext.Current.SessionId);
        }
        
        
        public IEnumerable<Customer> GetCustomers(RequestFilters filters)
        {
            try
            {
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, OperationContext.Current.SessionId, "GetCustomers");

                var fn = filters.FirstName;
                var ln = filters.LastName;
                var ret = new List<Customer>();

                // Copy the values. Better be safe than sorry.
                var vals = SessionManager.CustomerDetailsById.Values.ToList();
                foreach (var c in vals)
                {
                    var hit = (fn == "" || String.Compare(c.FirstName.Substring(0, fn.Length), fn, true) == 0 &&
                    (ln == "" || String.Compare(c.LastName.Substring(0, ln.Length), ln, true) == 0));

                    if (hit)
                    {
                        ret.Add(c.CloneToCustomer());
                    }
                }
                             
                return ret;
            }
            catch(Exception ex)
            {
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, OperationContext.Current.SessionId, ex.Message);
                SendNotification(Notification.UnexpectedError);
                return new List<Customer>() ;
            }
            
        }
        public CustomerDetails GetCustomerDetails(int Id)
        {
            try
            {
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, OperationContext.Current.SessionId, "GetCustomerDetails");
                CustomerDetails ret;
                SessionManager.CustomerDetailsById.TryGetValue(Id, out ret);

                return ret;
            }
            catch(Exception ex)
            {
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, OperationContext.Current.SessionId, ex.Message);
                SendNotification(Notification.UnexpectedError);
                return null;
            }
        }
        public bool DeleteCustomer(int Id)
        {
            try
            {
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, OperationContext.Current.SessionId, "DeleteCustomer " + Id.ToString());
                CustomerDetails v;
                bool r = SessionManager.CustomerDetailsById.TryRemove(Id, out v);

                if (r)
                {
                    SendNotification(Notification.RecordDeleted);
                }
                return r;
            }
            catch(Exception ex)
            {
                SendNotification(Notification.UnexpectedError);
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, OperationContext.Current.SessionId, ex.Message);
                return false;
            }
        }
        public bool UpdateCustomer(CustomerDetails customerDetails)
        {
            try
            {
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, 
                    OperationContext.Current.SessionId, "DeleteCustomer " + customerDetails.Id.ToString());

                CustomerDetails v;
                bool r = SessionManager.CustomerDetailsById.TryRemove(customerDetails.Id, out v);
                r &= SessionManager.CustomerDetailsById.TryAdd(customerDetails.Id, customerDetails);
                SendNotification(Notification.RecordUpdated);
                return r;
            }
            catch (Exception ex)
            {
                SendNotification(Notification.UnexpectedError);
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, OperationContext.Current.SessionId, ex.Message);
                return false;
            }
        }
        public bool AddNewCustomer(CustomerDetails customerDetails)
        {
            try
            {
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, OperationContext.Current.SessionId, "AddNewCustomer");
                var id = SessionManager.CustomerDetailsById.Values.Max(o => o.Id);
                customerDetails.Id = id + 1;
                SessionManager.CustomerDetailsById.TryAdd(customerDetails.Id, customerDetails);
                SendNotification(Notification.RecordAdded);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} session {1} , {2}", DateTime.Now, OperationContext.Current.SessionId, ex.Message);
                SendNotification(Notification.UnexpectedError);
                return false;
            }
            
        }

        private void SendNotification(Notification n)
        {
            object[] args = new object[] { _callBack, n };
            ThreadPool.QueueUserWorkItem(SessionManager.SendNotification, args);
            
        }
    }
}
