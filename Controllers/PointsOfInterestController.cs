using AutoMapper;
using CItyInfo.API.Entities;
using CItyInfo.API.Model;
using CItyInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace CItyInfo.API.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Authorize(Policy = "MustBeFromParis")]
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : Controller
    {
        
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly CitiesDataStore _dataStore; //TODO remove
        private readonly ICityInfoRespository _cityInfoRespository;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, CitiesDataStore dataStore, ICityInfoRespository cityInfoRespository, IMailService mailService, IMapper mapper) 
        { 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore)); //TODO remove
            _cityInfoRespository = cityInfoRespository ?? throw new ArgumentNullException(nameof(cityInfoRespository));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterestsForCityAsync(int cityId)
        {
            try
            {
                //var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
                //var cityMatches = await _cityInfoRespository.CityNameMatchesCityId(cityName, cityId);

                //if (!cityMatches)
                //    return Forbid();

                var cityExists = await _cityInfoRespository.CityExistsAsync(cityId);

                if (!cityExists)
                    return NotFound();

                var pointEntities = await _cityInfoRespository.GetPointsOfInterestsForCityAsync(cityId);

                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointEntities));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting city {cityId}", ex);
                return StatusCode(500, "Problem");
            }
        }

        [HttpGet("{pointId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointId)
        {
            var point = await _cityInfoRespository.GetPointOfInterestsForCityAsync(cityId, pointId);

            if (point == null)
                return NotFound();

            return Ok(_mapper.Map<PointOfInterestDto>(point));
        }
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
            int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            var cityExists = await _cityInfoRespository.CityExistsAsync(cityId);

            if (!cityExists)
                return NotFound();

            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

            await _cityInfoRespository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRespository.SaveChangesAsync();

            var createdPointOfInterest = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointId = createdPointOfInterest.Id
                },
                createdPointOfInterest
            );
        }

        [HttpPut("{pointId}")]
        public async Task<ActionResult<PointOfInterestDto>> UpdatePointOfInterest(
            int cityId, int pointId, PointOfInterestForUpdateDto pointOfInterest)
        {
            var cityExists = await _cityInfoRespository.CityExistsAsync(cityId);

            if (!cityExists)
                return NotFound();

            var point = await _cityInfoRespository.GetPointOfInterestsForCityAsync(cityId, pointId);

            if (point == null)
                return NotFound();

            _mapper.Map(pointOfInterest, point);

            await _cityInfoRespository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{pointId}")]
        public async Task<ActionResult<PointOfInterestDto>> PatchPointOfInterest(
            int cityId, int pointId, 
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var cityExists = await _cityInfoRespository.CityExistsAsync(cityId);

            if (!cityExists)
                return NotFound();

            var point = await _cityInfoRespository.GetPointOfInterestsForCityAsync(cityId, pointId);

            if (point == null)
                return NotFound();

            var pointToPatch = _mapper.Map<PointOfInterestForUpdateDto>(point);

            patchDocument.ApplyTo(pointToPatch, ModelState);

            if(!ModelState.IsValid || !TryValidateModel(pointToPatch))
                return BadRequest(ModelState);

            _mapper.Map(pointToPatch, point);

            await _cityInfoRespository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointId)
        {
            var cityExists = await _cityInfoRespository.CityExistsAsync(cityId);

            if (!cityExists)
                return NotFound();

            var point = await _cityInfoRespository.GetPointOfInterestsForCityAsync(cityId, pointId);

            if (point == null)
                return NotFound();

            _cityInfoRespository.DeletePointOfInterest(point);
            await _cityInfoRespository.SaveChangesAsync();

            _mailService.Send("Point removed");

            return NoContent();

        }
    }
}
