using CItyInfo.API.DbContexts;
using CItyInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CItyInfo.API.Services
{
    public class CityInfoRespository : ICityInfoRespository
    {
        private readonly CityInfoContext _context;

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Cities as IQueryable<City>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery) || (a.Description != null && a.Description.Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {

            if(includePointsOfInterest) 
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();

            return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestsForCityAsync(int cityId, int pointId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsForCityAsync(int cityId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }

        public CityInfoRespository(CityInfoContext cityInfoContext)
        {
            _context = cityInfoContext ?? throw new ArgumentNullException(nameof(cityInfoContext));
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);

            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }
    }
}
