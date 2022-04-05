using Microsoft.AspNetCore.Authorization;
using Zhetistik.Data.ViewModels;

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/achievements")]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IAchievementTypeRepository _achievementTypeRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AchievementController> _logger;

        public AchievementController(IAchievementRepository achievementRepository, IAchievementTypeRepository achievementTypeRepository, IFileRepository fileRepository, IPortfolioRepository portfolioRepository, IWebHostEnvironment env, ILogger<AchievementController> logger)
        {
            _achievementRepository = achievementRepository;
            _achievementTypeRepository = achievementTypeRepository;
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
        public async Task<ActionResult<AchievementViewModel>> GetAchievementAsync(int id)
        {
            var existingAchievement = await _achievementRepository.GetAsync(id);
            if(existingAchievement is null)
            {
                return NotFound();
            }
            var achievementViewModel = new AchievementViewModel();
            achievementViewModel.AchievementId = id;
            achievementViewModel.AchievementName = existingAchievement.AchievementName;
            achievementViewModel.Description = existingAchievement.Description;
            achievementViewModel.FileModel = existingAchievement.FileModel;
            achievementViewModel.AchievementTypeName = (await _achievementTypeRepository.GetAsync(existingAchievement.AchievementTypeId)).AchievementTypeName;
            return achievementViewModel;
        }
        [HttpGet("portfolio")]
        public async Task<IEnumerable<AchievementViewModel>> GetAchievementsByCandidateAsync(int candidateId)
        {
            var achievements = await _achievementRepository.GetAchievementsByCandidateAsync(candidateId);
            return achievements;
        }
        [HttpPost]
        public async Task<ActionResult<AchievementViewModel>> AddAchievementAsync(int portfolioId, [FromForm]CreateAchievementViewModel achievementViewModel)
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
                    var directory = "Achievements/" + portfolioId + '/';
                    //home/effuone/Desktop/7stik-Service/Zhetistik.Api/Achievements/PortfolioId
                    achievement.FileModel = await _fileRepository.SaveFileAsync(env, directory, achievementViewModel.File);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new Response {Status = "Fail", Message = "This file already exists, or change fileName"});
                }
            }
            achievement.Description = achievementViewModel.Description;
            achievement.PortfolioId = portfolioId;
            await _achievementRepository.CreateAsync(achievement);
            var newAchievementViewModel = new AchievementViewModel();
            newAchievementViewModel.AchievementId = achievement.AchievementId;
            newAchievementViewModel.AchievementTypeName = (await _achievementTypeRepository.GetAsync(achievement.AchievementTypeId)).AchievementTypeName;
            newAchievementViewModel.Description = achievement.Description;
            newAchievementViewModel.FileModel = achievement.FileModel;
            return CreatedAtAction(nameof(GetAchievementAsync), new { id = achievement.AchievementId }, newAchievementViewModel);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteAchievementAsync(int portfolioId, int achievementId)
        {
            var achievement = await _achievementRepository.GetAsync(achievementId);
            if(achievement is null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var portfolio = await _portfolioRepository.GetAsync(portfolioId);
            if(portfolio is null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var achievements = await _achievementRepository.GetAchievementsByPortfolioAsync(portfolioId);
            var file = await _fileRepository.GetFileAsync(achievement.FileModel.Id);
            if(file is not null)
            {
                await _fileRepository.DeleteFileAsync(file.Id);
            }
            else
            {
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "File doesn't exist"});
            }
            return StatusCode(StatusCodes.Status204NoContent, new Response { Status = "Success", Message = "Deleted achievement"});
        }
    }
}