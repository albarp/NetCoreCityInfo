using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreCityInfo.Models;
using NetCoreCityInfo.Services;

namespace NetCoreCityInfo.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                //throw new ApplicationException("Err");

                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City {cityId} was not found.");

                    return NotFound();
                }


                return Ok(city.PointsOfInterest);
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Error happend while handling GetPointsOfInterest for city: {cityId}. {ex}");

                return StatusCode(500, "An error happend while handling your request");
            }
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        // Aggiunge una risorsa
        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointsOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if(pointOfInterest == null)
            {
                return BadRequest();
            }

            // Per gestire errori personalizzati, va popolato il model state secondo le proprie logiche
            // Ovviamente non è il massimo, perchè la validazione è fatta un po' nel DTO e un po' nel controller
            // Nel corso si suggerisce di usare FluentValidation
            if(pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "Description must be different than name");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if(city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            // in pratica recupera il route template GetPointOfInterest, che abbiamo definito un paio di metodi prima, e 
            // valorizza i parmetri cityId e id con un oggetto anonimo
            // Infine mette finalPointOfInterest nel body
            return CreatedAtRoute("GetPointOfInterest", new { cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        // Aggiorna una risorsa
        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            // Per gestire errori personalizzati, va popolato il model state secondo le proprie logiche
            // Ovviamente non è il massimo, perchè la validazione è fatta un po' nel DTO e un po' nel controller
            // Nel corso si suggerisce di usare FluentValidation
            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "Description must be different than name");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if(pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        // Specifiche delle operazioni Json patch: https://tools.ietf.org/html/rfc6902
        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartialUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if(patchDoc == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch =
                new PointOfInterestForUpdateDto
                {
                    Description = pointOfInterestFromStore.Description,
                    Name = pointOfInterestFromStore.Name
                };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            if(pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different than the name");
            }

            // Va chiamato a mano
            // perchè il framewrok valida il parametro in ingresso: JsonPatchDocument
            // e non PointOfInterestForUpdateDto, quindi le data annotations vanno a farsi benedire
            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();

        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            _mailService.Send("Point of interest deleted", 
                $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted." );

            return NoContent();
        }

        // test
        [HttpPost("load")]
        public IActionResult TextXml([FromBody]XElement o)
        {
            var attr =
                from el in o.Elements("content")
                where (string)el.Attribute("a") == "xxx"
                select el;

            foreach (XElement el in attr)
                Console.WriteLine(el);

            return Ok();
        }
    }
}