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
