using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLernionService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace QLernionService
{
    internal  class DBContextUtils
    {
        private static QLernionDBConext _dbContext;
        private static bool _inMemory;

        public static void InitDBContext(IConfiguration config)
        {
            var inMemory = config.GetValue<byte>("DbInMemory");
            if (inMemory == 1)
            {
                _inMemory = true;
                var options = new DbContextOptionsBuilder<QLernionDBConext>()
                   .UseInMemoryDatabase(databaseName: "QLernionDB")
                   .Options;
                _dbContext = new QLernionDBConext(options);
                
            }
            
            
        }

        public static QLernionDBConext GetDBContext2()
        {
            
            if (_inMemory)
            {

                var options = new DbContextOptionsBuilder<QLernionDBConext>()
                   .UseInMemoryDatabase(databaseName: "QLernionDB")
                   .Options;
                return  new QLernionDBConext(options);

            }
            else
            {
                return new QLernionDBConext();
            }
        }

        public static void InitDBContext()
        {
            
            if (_dbContext != null)
            {

                var options = new DbContextOptionsBuilder<QLernionDBConext>()
                   .UseInMemoryDatabase(databaseName: "QLernionDB")
                   .Options;
                _dbContext = new QLernionDBConext(options);

            }

        }
        public static QLernionDBConext GetDBContext()
        {           
            if(_dbContext != null)
            {

                Monitor.Enter(_dbContext);
                return _dbContext;
            }
            else
            {
                return new QLernionDBConext();
            }
        }

        public static void ReleaseDBContext()
        {
            
            if (_dbContext != null)
            {
                try
                {
                    Monitor.Exit(_dbContext);
                }
                catch
                {

                }
            }
        }
    }
}
