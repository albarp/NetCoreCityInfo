using NetCoreCityInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreCityInfo
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public List<CityDto> Cities{ get; set; }

        public CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto
                {
                    Id = 1,
                    Name = "New York",
                    Description = "The one with the big park"
                },
                new CityDto
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "The one with the cathedral that wa really never finished"
                },
                new CityDto
                {
                    Id  = 3,
                    Name = "Paris",
                    Description = "The one with the big tower"
                }
            };
        }
    }
}
