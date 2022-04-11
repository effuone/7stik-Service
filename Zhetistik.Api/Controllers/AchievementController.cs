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
        public async Task<ActionResult<Achievement>> AddAchievementAsync(int portfolioId, [FromForm]CreateAchievementViewModel achievementViewModel)
        {
            //check wheteher portfolio exists
            //create new achievement
            //map it
            //save it
            //display it
            var existingPortfolio = await _portfolioRepository.GetAsync(portfolioId);
            if(existingPortfolio is null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var achievement = new Achievement();
            achievement.AchievementName = achievementViewModel.AchievementName;
            achievement.AchievementTypeId = achievementViewModel.AchievementTypeId;
            achievement.Description = achievementViewModel.Description;
            string newPath = @"Achievements\" + portfolioId + @"\";
            var fileId = await _fileRepository.SaveFileAsync(_env.ContentRootPath, newPath, achievementViewModel.File);
            achievement.FileModel = await _fileRepository.GetFileAsync(fileId);
            var id = await _achievementRepository.CreateAsync(achievement);
            return CreatedAtAction(nameof(GetAchievementAsync), new { id = id }, achievement);
        }
        // [HttpDelete]
        // public async Task<ActionResult> DeleteAchievementAsync(int achievementId)
        // {
        //     //удаляется достижение
        //     //удаляется файл
        // }
    }
}