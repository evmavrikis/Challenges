using NReco.Logging.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

namespace QLernionService
{
    public  class ServiceUtils
    {
        private const string LOG_FILE = "QLernionLog.txt";
        private static LoggerFactory _loggerFactory;
        
        public static ILogger CreateLogger(IConfiguration config, string category)
        {
            var filePath = LOG_FILE;
            var f = config.GetValue<string>("LogFilePath");
            if (f != null && f!="")
            {
                filePath = f;
            }
            return CreateLogger(filePath, category);
        }

        public static ILogger CreateLogger(string category)
        {
            return CreateLogger(LOG_FILE, category);
        }

        private static ILogger CreateLogger(string filePath, string category)
        {
            
           
            var logger = LoggerFactory.CreateLogger(category);
            return logger;

           
        }

        // This is introduced for unit tests. No multithreading risk.
        private static LoggerFactory LoggerFactory
        {
            get
            {
                if (_loggerFactory == null)
                {
                    var filePath = LOG_FILE;
                    _loggerFactory = new LoggerFactory();
                    _loggerFactory.AddProvider(new FileLoggerProvider(filePath));
                }
                return _loggerFactory;
            }
        }
            


        public static void InitLogger(IConfiguration config)
        {
            var filePath = LOG_FILE;
            var f = config.GetValue<string>("LogFilePath");
            if (f != null && f != "")
            {
                filePath = f;
            }
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddProvider(new FileLoggerProvider(filePath));
        }

       

        
    }
}
