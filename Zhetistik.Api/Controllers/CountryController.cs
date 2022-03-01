

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _repository;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryRepository repository, ILogger<CountryController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            return await _repository.GetAllAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountryAsync(int id)
        {
            var existingModel = await _repository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound("Country not found");
            }
            return existingModel;
        }
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountryAsync([Required]string countryName)
        {
            var existingModel = await _repository.GetAsync(countryName);
            if(existingModel is not null)
            {
                return BadRequest("Country already exists");
            }
            var model = new Country();
            model.CountryName = countryName;
            model.CountryId = await _repository.CreateAsync(model);
             return CreatedAtAction(nameof(GetCountryAsync), new { id = model.CountryId }, model);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UdpateCountryAsync(int id, [Required] string countryName)
        {
            var existingModel = await _repository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound("Country not found");
            }
            var result = await _repository.UpdateAsync(id, new Country(){CountryName = countryName});
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCountryAsync(int id)
        {
            var existingModel = await _repository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound("Country not found");
            }
            var result = await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}