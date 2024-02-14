using CItyInfo.API.Model;
using System.Xml.Linq;

namespace CItyInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }

        // public static CitiesDataStore Current { get; } = new CitiesDataStore();       

        public CitiesDataStore ()
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "The one with the big park",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "A",
                            Description = "A",
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "B",
                            Description = "B",
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "The one with the cathedral",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 3,
                            Name = "A",
                            Description = "A",
                        },
                        new PointOfInterestDto()
                        {
                            Id = 4,
                            Name = "B",
                            Description = "B",
                        }
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with the tower",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 5,
                            Name = "A",
                            Description = "A",
                        },
                        new PointOfInterestDto()
                        {
                            Id = 6,
                            Name = "B",
                            Description = "B",
                        }
                    }
                }
            };

        }
    }
}
