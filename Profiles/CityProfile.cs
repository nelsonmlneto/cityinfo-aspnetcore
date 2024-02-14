using AutoMapper;

namespace CItyInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile() 
        {
            CreateMap<Entities.City, Model.CityWithoutPointsOfInterestDto>();
            CreateMap<Entities.City, Model.CityDto>();
        }
    }
}
