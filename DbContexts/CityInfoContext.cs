using CItyInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CItyInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; }

        public DbSet<PointOfInterest> PointOfInterests { get; set; }

        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "The one with the big park",
                },
                new City("Antwerp")
                {
                    Id = 2,
                    Description = "The one with the cathedral",
                },
                new City("Paris")
                {
                    Id = 3,
                    Description = "The one with the tower",
                });

            modelBuilder.Entity<PointOfInterest>().HasData(
                new PointOfInterest("A")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "A",
                },
                new PointOfInterest("B")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "B",
                },
                new PointOfInterest("C")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "C",
                },
                new PointOfInterest("E")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "D",
                },
                new PointOfInterest("E")
                {
                    Id = 5,
                    CityId = 3,
                    Description = "E",
                },
                new PointOfInterest("E")
                {
                    Id = 6,
                    CityId = 3,
                    Description = "E",
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
