using Microsoft.AspNetCore.Authorization;
using Zhetistik.Data.ViewModels;

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/achievements")]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AchievementController> _logger;

        public AchievementController(IAchievementRepository achievementRepository, IFileRepository fileRepository, IPortfolioRepository portfolioRepository, IWebHostEnvironment env, ILogger<AchievementController> logger)
        {
            _achievementRepository = achievementRepository;
            _fileRepository = fileRepository;
            _portfolioRepository = portfolioRepository;
            _env = env;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IEnumerable<Achievement>> GetAchievementsAsync()
        {
            return await _achievementRepository.GetAllAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Achievement>> GetAchievementAsync(int id)
        {
            var existingAchievement = await _achievementRepository.GetAsync(id);
            if(existingAchievement is null)
            {
                return NotFound();
            }
            return existingAchievement;
        }
        [HttpGet("portfolio")]
        public async Task<IEnumerable<AchievementViewModel>> GetAchievementsByCandidateAsync(int candidateId)
        {
            var achievements = await _achievementRepository.GetAchievementsByCandidateAsync(candidateId);
            return achievements;
        }
        [HttpPost]
        public async Task<ActionResult<Achievement>> AddAchievementAsync(int portfolioId, [FromForm]CreateAchievementViewModel achievementViewModel)
        {
            var existingPortfolio = await _portfolioRepository.GetAsync(portfolioId);
            if(existingPortfolio is null)
            {
                return NotFound();
            }
            var achievement = new Achievement();
            achievement.AchievementName = achievementViewModel.AchievementName;
            achievement.AchievementTypeId = achievementViewModel.AchievementTypeId;
            if(achievementViewModel.File is not null)
            {
                var existingFile = await _fileRepository.GetFileAsync(achievementViewModel.File.FileName);
                if(existingFile is null)
                {
                    var env = _env.ContentRootPath;
                    var achievementFolder = @$"/Achievements/{existingPortfolio.PortfolioId}/";
                    var directory = env + achievementFolder;
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var physicalPath = $"/{achievementFolder}/";
                    achievement.FileModel = await _fileRepository.SaveFileAsync(physicalPath, achievementViewModel.File);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new Response {Status = "Fail", Message = "This file already exists, or change fileName"});
                }
            }
            achievement.Description = achievementViewModel.Description;
            achievement.PortfolioId = portfolioId;
            await _achievementRepository.CreateAsync(achievement);
            return CreatedAtAction(nameof(GetAchievementAsync), new { id = achievement.AchievementId }, achievement);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAchievementAsync(int portfolioId, int achievementId)
        {
            var existingAchievement = await _achievementRepository.GetAsync(achievementId);
            if(existingAchievement is null)
            {
                return NotFound();
            }
            var achievementFolder = @$"/Achievements/{portfolioId}/";
            var file = existingAchievement.FileModel;
            try    
            {    
                // Check if file exists with its full path    
                if (System.IO.File.Exists(Path.Combine(achievementFolder, file.FileName)))    
                {    
                    // If file found, delete it    
                    System.IO.File.Delete(Path.Combine(achievementFolder, file.FileName));    
                }      
            }    
            catch (IOException ioExp)    
            {    
                 Console.WriteLine(ioExp.Message);    
            }  
            await _achievementRepository.DeleteAsync(achievementId);
            if(file is not null)
            {
                await _fileRepository.DeleteFileAsync(existingAchievement.FileModel.Id);
            }
            return StatusCode(StatusCodes.Status204NoContent, new Response { Status = "Success", Message = "Deleted achievement"});
        }
    }
}