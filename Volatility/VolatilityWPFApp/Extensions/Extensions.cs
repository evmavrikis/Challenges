using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatilityContracts;
using System.Reflection;

namespace VolatilityWPFApp.Extensions
{
    internal static class Extensions
    {
        /// <summary>
        /// Initialises details from the main customer record. It is used by the mock and for adding new customers.
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="source"></param>
        public static void CopyFrom(this CustomerDetails dest, CustomerDetails source )
        {
            var props = dest.GetType().GetProperties();
            foreach (var p in props)
            {
                var v = p.GetValue(source);
                p.SetValue(dest, v);
            }
        }
    }
}
