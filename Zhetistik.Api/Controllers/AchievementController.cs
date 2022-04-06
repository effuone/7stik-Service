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
        // {
        //     "achievementId": 1,
        //     "portfolioId": 1,
        //     "achievementName": "SAT",
        //     "description": "1600",
        //     "fileModel": null, 
        //     "achievementType": null 
        // }
        //TODO: fix file model
        //TODO: fix achievementType
        [HttpGet]
        public async Task<IEnumerable<Achievement>> GetAchievementsAsync()
        {
            return await _achievementRepository.GetAllAsync();
        }
        [HttpGet("vm")]
        public async Task<IEnumerable<AchievementViewModel>> GetAchievementsViewModelsAsync()
        {
            return await _achievementRepository.GetAllAchievementViewModelsAsync();
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
            achievementViewModel.FilePath = existingAchievement.FileModel.Path;
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
            newAchievementViewModel.FilePath = achievement.FileModel.Path;
            return CreatedAtAction(nameof(GetAchievementAsync), new { id = achievement.AchievementId }, newAchievementViewModel);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteAchievementAsync(int achievementId)
        {
            var achievement = await _achievementRepository.GetAsync(achievementId);
            if(achievement is null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            await _fileRepository.DeleteFileAsync(achievement.FileModel.Id);
            await _achievementRepository.DeleteAsync(achievementId);
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}