using AutoMapper;

namespace CItyInfo.API.Profiles
{
    public class PointOfInterestProfile : Profile
    {
        public PointOfInterestProfile() 
        {
            CreateMap<Entities.PointOfInterest, Model.PointOfInterestDto>();
            CreateMap<Model.PointOfInterestForCreationDto, Entities.PointOfInterest>();
            CreateMap<Model.PointOfInterestForUpdateDto, Entities.PointOfInterest>();
            CreateMap<Entities.PointOfInterest, Model.PointOfInterestForUpdateDto>();
        }
    }
}
