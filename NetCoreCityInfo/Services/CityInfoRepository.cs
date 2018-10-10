using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreCityInfo.Entities;

namespace NetCoreCityInfo.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            // Vabbè, non mi fa impazzire, controlla che la città ci sia in un altro metodo, quindi questo potrebbe scoppiare
            var city = GetCity(cityId, false);

            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool CityExist(int cityId)
        {
            return _context.Cities.Any(c => c.Id == cityId);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfinterest)
        {
            _context.PointsOfInterest.Remove(pointOfinterest);
        }

        public IEnumerable<City> GetCities()
        {
            // Chiamare ToList fa eseguire la query vera e propria
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointOfInterest)
        {
            if (includePointOfInterest)
            {
                return _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefault();
            }

            // In questo caso è la chiamata a FirstOrDefault che scatena la query
            return _context.Cities.Where(c => c.Id == cityId).FirstOrDefault();
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _context
                .PointsOfInterest
                .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                .FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _context.PointsOfInterest.Where(p => p.CityId == cityId).ToList();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
