using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreCityInfo.Controllers
{
    [Route("api/diagnostics")]
    public class DiagnosticsController : Controller
    {
        IHostingEnvironment _env;

        public DiagnosticsController(IHostingEnvironment env)
        {
            _env = env;
        }
        [HttpGet()]
        public IActionResult GetDiagnostics()
        {
            return Ok(_env.EnvironmentName);
        }
    }
}