using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QLernionService.Extensions;

namespace QLernionService.Controllers
{
    public class ErrorController : Controller
    {
        private IConfiguration _config;
        public ErrorController(IConfiguration config)
        {
            _config = config;
        }
        
        [Route("/error")]
        public IActionResult Error()
        {
            var logger = ServiceUtils.CreateLogger(_config, "ERROR");
            var p = Problem();
            logger.LogErrorInBackground(p.StatusCode.ToString());
            return p;
        }
    }
}
