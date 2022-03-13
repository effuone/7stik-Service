using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
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
        public async Task<IEnumerable<Achievement>> GetAchievementsByPortfolioAsync(int portfolioId)
        {
            var achievements = await _achievementRepository.GetAchievementsByPortfolioAsync(portfolioId);
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
            achievement.FileModel = await _fileRepository.SaveFileAsync(achievementViewModel.File);
            achievement.Description = achievementViewModel.Description;
            achievement.PortfolioId = portfolioId;
            await _achievementRepository.CreateAsync(achievement);
            return CreatedAtAction(nameof(GetAchievementAsync), new { id = achievement.AchievementId }, achievement);
            // var achievement = new Achievement();
            // achievement.AchievementName = achievementViewModel.AchievementName;
            // achievement.AchievementTypeId = achievementViewModel.AchievementTypeId;
            // if (achievementViewModel.File != null)
            // {
            //     var uploadedFile = achievementViewModel.File;
            //     // путь к папке Files
            //     string path = "/AchievementFiles/" + uploadedFile.FileName;
            //     // сохраняем файл в папку Files в каталоге wwwroot
            //     using (var fileStream = new FileStream(_env.WebRootPath + path, FileMode.Create))
            //     {
            //         await uploadedFile.CopyToAsync(fileStream);
            //     }
            //     FileModel file = new FileModel { FileName = uploadedFile.FileName, FilePath = path};
            // }
            // achievement.PortfolioId = portfolioId;
            // await _achievementRepository.CreateAsync(achievement);
            // return CreatedAtAction(nameof(GetAchievementAsync), new { id = achievement.AchievementId }, achievement);
        }
    }
}