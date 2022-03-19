using Microsoft.AspNetCore.Authorization;
using Zhetistik.Data.ViewModels;
namespace Zhetistik.Api.Controllers
{
    [Route("api/schools")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SchoolController> _logger;

        public SchoolController(ISchoolRepository schoolRepository, ILocationRepository locationRepository, IWebHostEnvironment env, ILogger<SchoolController> logger)
        {
            _schoolRepository = schoolRepository;
            _locationRepository = locationRepository;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<School>> GetSchoolsAsync()
        {
            return await _schoolRepository.GetAllAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<School>> GetSchoolAsync(int id)
        {
            var existingModel = await _schoolRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound();   
            }
            return existingModel;
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<School>> PostSchoolAsync(CreateSchoolViewModel schoolViewModel)
        {
            var existingLocation = await _locationRepository.GetAsync(schoolViewModel.LocationId);
            if(existingLocation is null)
            {
                return NotFound("Location not found");
            }
            var existingModel = await _schoolRepository.GetAsync(schoolViewModel.SchoolName);
            if(existingModel is not null)
            {
                return BadRequest("School already exists");
            }
            var model = new School();
            model.LocationId = schoolViewModel.LocationId;
            model.SchoolName = schoolViewModel.SchoolName;
            model.Image = schoolViewModel.Image;
            model.FoundationYear = schoolViewModel.FoundationYear;
            model.SchoolId = await _schoolRepository.CreateAsync(model);
             return CreatedAtAction(nameof(GetSchoolAsync), new { id = model.SchoolId }, model);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateSchoolAsync(int id, UpdateSchoolViewModel schoolViewModel)
        {
            var existingModel = await _schoolRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound("School not found");
            }
            var model = new School();
            model.LocationId = schoolViewModel.LocationId;
            model.Image = schoolViewModel.Image;
            model.FoundationYear = schoolViewModel.FoundationYear;
            model.SchoolName = schoolViewModel.SchoolName;
            var result = await _schoolRepository.UpdateAsync(id, model);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCountryAsync(int id)
        {
            var existingModel = await _schoolRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound("School not found");
            }
            var result = await _schoolRepository.DeleteAsync(id);
            return NoContent();
        }
        [HttpPost]
        [Route("/api/schools/images")]
        public async Task<string> PostImageAsync(IFormFile objFile)
        {
            try
            {
                var uploadPath = _env.WebRootPath+"/SchoolImages/";
                if(objFile.Length > 0)
                {
                    if(!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    using(FileStream fileStream = System.IO.File.Create(uploadPath+objFile.FileName))
                    {
                        await objFile.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                        return objFile.FileName;
                    }
                }
                else
                {
                    return "Failed";
                }
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }
            
        }
    }
}