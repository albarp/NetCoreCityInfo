using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreCityInfo.Entities;

namespace NetCoreCityInfo.Controllers
{
    public class DummyController : Controller
    {
        private CityInfoContext _ctx;

        // Questo è solo un controller utilizzato per creare il db entity framework in modo automatico
        // In pratica, quando il controller viene istanziato, viene chiamato il costruttore che
        // si aspetta il CityInfoContext. CityInfoContext è quindi creato dal container.
        // Il costruttore di CityInfoContext ha, al suo interno, l'istruzione per creare
        // il db

        public DummyController(CityInfoContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        [Route("api/testdatabase")]
        public IActionResult TestDatabase()
        {
            return Ok();
        }
    }
}