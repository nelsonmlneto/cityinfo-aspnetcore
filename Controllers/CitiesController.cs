using AutoMapper;
using CItyInfo.API.Model;
using CItyInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CItyInfo.API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRespository _cityInfoRespository;
        private readonly IMapper _mapper;
        const int MAX_CITIES_PAGE_SIZE = 20;

        public CitiesController(ICityInfoRespository cityInfoRespository, IMapper mapper) {
            _cityInfoRespository = cityInfoRespository ?? throw new ArgumentNullException(nameof(cityInfoRespository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > MAX_CITIES_PAGE_SIZE)
                pageSize = MAX_CITIES_PAGE_SIZE;

            var (cityEntities, pagination) = await _cityInfoRespository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
            
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }


        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="id">The id of the city to get</param>
        /// <param name="includePointsOfInterest">Whether or not to include the points of interest</param>
        /// <returns>An IActionResult</returns>
        /// <response code="200">Returns the requested city</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] //doc
        [ProducesResponseType(StatusCodes.Status404NotFound)] //doc
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //doc
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRespository.GetCityAsync(id, includePointsOfInterest);

            if (city == null)
                return NotFound();

            if (includePointsOfInterest)
                return Ok(_mapper.Map<CityDto>(city));

            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
