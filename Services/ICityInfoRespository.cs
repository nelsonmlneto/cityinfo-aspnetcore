using CItyInfo.API.Entities;

namespace CItyInfo.API.Services
{
    public interface ICityInfoRespository
    {
        public Task<IEnumerable<City>> GetCitiesAsync();

        public Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

        public Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);

        public Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsForCityAsync(int cityId);

        public Task<PointOfInterest?> GetPointOfInterestsForCityAsync(int cityId, int pointId);

        public Task<bool> CityExistsAsync(int cityId);

        public Task  AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

        public Task<bool> SaveChangesAsync();

        public void DeletePointOfInterest(PointOfInterest pointOfInterest);

        public Task<bool> CityNameMatchesCityId(string? cityName, int cityId);
    }
}
