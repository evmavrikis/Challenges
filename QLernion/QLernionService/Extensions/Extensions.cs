using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace QLernionService.Extensions
{
    public static class Extensions
    {
        public static string CreateRequestLogInfo(this HttpRequest req, string data)
        {
            if (req == null)
            {
                return string.Format("Req time {0}, Test mode ", DateTime.UtcNow);
            }

            var ret = "";
            if (data != null &&  data.Length > 100)
            {
                data = data.Substring(0, 100);
            }
            else if (data == null || data == "")
            {
                data = "(none)";
            }
            ret += string.Format("Req time {0}, Client {1}:{2}, {3}, url {4}{5}, data {6}", DateTime.UtcNow, req.HttpContext.Connection.RemoteIpAddress,
                req.HttpContext.Connection.RemotePort, req.Method, req.Host.Value, req.Path, data);
            return ret;
        }

        
        public static void LogInfoInBackground(this ILogger logger, string text)
        {
            Task.Run(() => logger.LogInformation(text));
        }

        public static void LogErrorInBackground(this ILogger logger,  string text)
        {
            
            Task.Run(() => logger.LogError(text));
           
        }
    }
}
