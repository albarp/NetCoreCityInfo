using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using NetCoreCityInfo.Services;

namespace NetCoreCityInfo.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            // In Memory store
            //return Ok(CitiesDataStore.Current.Cities);

            var cities = AutoMapper.Mapper.Map<IEnumerable<Models.CityWithoutPointOfInterestDto>>(
                _cityInfoRepository.GetCities()
            );

            return Ok(cities);

        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointOfInterest = false)
        {
            //var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            //if(cityToReturn == null)
            //{
            //    return NotFound();
            //}

            //return Ok(cityToReturn);

            var city = _cityInfoRepository.GetCity(id, includePointOfInterest);

            if(city == null)
            {
                return NotFound();
            }

            if (includePointOfInterest)
            {
                var cityResult = AutoMapper.Mapper.Map<Models.CityDto>(city);
                return Ok(cityResult);
            }

            var cityWithoutPointsOfInterestResult = AutoMapper.Mapper.Map<Models.CityWithoutPointOfInterestDto>(city);
            return Ok(cityWithoutPointsOfInterestResult);
        }
    }
}
