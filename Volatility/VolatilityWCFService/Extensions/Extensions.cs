using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatilityContracts;
using System.Reflection;

namespace VolatilityWCFService.Extensions
{
    internal static class Extensions
    {
        public static Customer CloneToCustomer(this CustomerDetails cd)
        {
            var props = (typeof(Customer)).GetProperties();
            var ret = new Customer();

            foreach (var p in props)
            {
                var v = p.GetValue(cd);
                p.SetValue(ret, v);
            }
            
            return ret;
        }
    }
}
