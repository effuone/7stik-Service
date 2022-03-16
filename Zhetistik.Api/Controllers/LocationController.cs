using Microsoft.AspNetCore.Authorization;
using Zhetistik.Data.ViewModels;

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILocationRepository locationRepository, ICityRepository cityRepository, ICountryRepository countryRepository, ILogger<LocationController> logger)
        {
            _locationRepository = locationRepository;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _logger = logger;
        }

        [Route("/api/locations/vm/")]
        [HttpGet]
        public async Task<IEnumerable<LocationViewModel>> GetAllLocationViewModelsAsync()
        {
            return await _locationRepository.GetAllViewModelsAsync();
        }
        [HttpGet]
        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            return await _locationRepository.GetAllAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocationAsync(int id)
        {
            var existingModel = await _locationRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound("Location not found");
            }
            return existingModel;
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Location>> PostLocationAsync(int countryId, int cityId)
        {
            var existingCity = await _cityRepository.GetAsync(cityId);
            var existingCountry = await _countryRepository.GetAsync(countryId);
            if(existingCity is null || existingCountry is null)
            {
                return NotFound("Country or city not found");
            }
            var existingModel = await _locationRepository.GetAsync(countryId, cityId);
            if(existingModel is not null)
            {
                return BadRequest("Location already exists");
            }
            var model = new Location();
            model.CityId = cityId;
            model.CountryId = countryId;
            model.LocationId = await _locationRepository.CreateAsync(model);
             return CreatedAtAction(nameof(GetLocationAsync), new { id = model.LocationId }, model);
        }
        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> UpdateLocationAsync(int id, int countryId, int cityId)
        {
            var existingModel = await _locationRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound("Location not found");
            }
            var result = await _locationRepository.UpdateAsync(id, new Location(){CountryId = countryId, CityId = cityId});
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> DeleteLocationAsync(int id)
        {
            var existingModel = await _locationRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound("Location not found");
            }
            var result = await _locationRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}