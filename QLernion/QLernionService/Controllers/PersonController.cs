using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using QLernionService.Models;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using QLernionService.Extensions;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("QLernionControllerTests")]
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace QLernionService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {

        private QLernionDBConext _dbContext;
        private ILogger _logger;

        public PersonController(IConfiguration config)
        {
            if (config.GetValue<byte>("DbInMemory") == 1)
            {
                
                var options = new DbContextOptionsBuilder<QLernionDBConext>()
                   .UseInMemoryDatabase(databaseName: "QLernionDB")
                   .Options;
                _dbContext = new QLernionDBConext(options);

            }
            else
            {
                _dbContext = new QLernionDBConext();
            }
            _logger = ServiceUtils.CreateLogger(config, "INFO");
        }

        internal PersonController(bool inMemory)
        {
            if (inMemory)
            {

                var options = new DbContextOptionsBuilder<QLernionDBConext>()
                   .UseInMemoryDatabase(databaseName: "QLernionDB")
                   .Options;
                _dbContext = new QLernionDBConext(options);

            }
            else
            {
                _dbContext = new QLernionDBConext();
            }
            _logger = ServiceUtils.CreateLogger("INFO");
        }
        internal PersonController(DbContext dbContext)
        {
            _dbContext = (QLernionDBConext)dbContext;
            _logger = ServiceUtils.CreateLogger("INFO");
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IEnumerable<Person> Get()
        {
            _logger.LogInfoInBackground(Request.CreateRequestLogInfo(""));

            var ret = _dbContext.People.ToList();
            var o = Request;
            return ret;
        }


        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public Person Get(int id)
        {
            _logger.LogInfoInBackground(Request.CreateRequestLogInfo(""));
            var ret = _dbContext.People.Find(id);
            return ret;
        }


        [HttpPost]
        
        [Authorize(AuthenticationSchemes = "Bearer")]
        public void Post([FromBody] Person value)
        {
            _logger.LogInfoInBackground(Request.CreateRequestLogInfo(JsonConvert.SerializeObject(value)));
            _dbContext.People.Add(value);
            _dbContext.SaveChanges();

        }

        
        
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public void Put(int id, [FromBody] Person value)
        {
            _logger.LogInfoInBackground(Request.CreateRequestLogInfo(JsonConvert.SerializeObject(value)));
            _dbContext.People.Update(value);
            _dbContext.SaveChanges();
            
        }


        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public void Delete(int id)
        {
            _logger.LogInfoInBackground(Request.CreateRequestLogInfo(""));
            var person = _dbContext.People.Find(id);
            if (person != null)
            {
                _dbContext.Remove(person);
                _dbContext.SaveChanges();
            }
            
        }
        

        
    }
}
