using Microsoft.AspNetCore.Authorization;

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/achievementTypes")]
    public class AchievementTypeController : ControllerBase
    {
        private readonly IAchievementTypeRepository _achievementTypeRepository;
        private readonly ILogger<AchievementTypeController> _logger;

        public AchievementTypeController(IAchievementTypeRepository achievementTypeRepository, ILogger<AchievementTypeController> logger)
        {
            _achievementTypeRepository = achievementTypeRepository;
            _logger = logger;
        }
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IEnumerable<AchievementType>> GetAchievementTypesAsync()
        {
            return await _achievementTypeRepository.GetAllAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AchievementType>> GetAchievementTypeAsync(int id)
        {
            var existingModel =  await _achievementTypeRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound();
            }
            return existingModel;
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<AchievementType>> PostAchievementTypeAsync(string achievementTypeName)
        {
            var existingAchievementType = await _achievementTypeRepository.GetAsync(achievementTypeName);
            if(existingAchievementType is not null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new Response {Status = "Error", Message="Achievement type already exists"});
            }
            var model = new AchievementType();
            model.AchievementTypeName = achievementTypeName;
            model.AchievementTypeId = await _achievementTypeRepository.CreateAsync(new AchievementType {AchievementTypeName = achievementTypeName});
            return CreatedAtAction(nameof(GetAchievementTypeAsync), new { id = model.AchievementTypeId }, model);
        }
        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> UpdateAchievementTypeAsync(int id, string achievementTypeName)
        {
            var existingModel =  await _achievementTypeRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound();
            }
            await _achievementTypeRepository.UpdateAsync(id, new AchievementType{AchievementTypeName = achievementTypeName});
            return StatusCode(StatusCodes.Status204NoContent, new Response { Status = "Success", Message = $"Updated achievement type {id}"});
        }
        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> DeleteAchievementTypeAsync(int id)
        {
            var existingModel =  await _achievementTypeRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound();
            }
            await _achievementTypeRepository.DeleteAsync(id);
            return StatusCode(StatusCodes.Status204NoContent, new Response { Status = "Success", Message = $"Deleted achievement type {id}"});
        }
    }
}