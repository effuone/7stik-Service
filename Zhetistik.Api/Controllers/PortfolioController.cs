using Microsoft.AspNetCore.Authorization;
using Zhetistik.Data.ViewModels;

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/portfolios")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IAchievementRepository _achievementRepository;
        private readonly IAchievementTypeRepository _achievementTypeRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(IPortfolioRepository portfolioRepository, IAchievementRepository achievementRepository, IAchievementTypeRepository achievementTypeRepository, ICandidateRepository candidateRepository, ILogger<PortfolioController> logger)
        {
            _portfolioRepository = portfolioRepository;
            _achievementRepository = achievementRepository;
            _achievementTypeRepository = achievementTypeRepository;
            _candidateRepository = candidateRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PortfolioViewModel>> GetPortfolioViewModelAsync(int id)
        {
            var portfolio = await _portfolioRepository.GetAsync(id);
            if(portfolio is null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var portfolioViewModel = new PortfolioViewModel();
            portfolioViewModel.PortfolioId = portfolio.PortfolioId;
            portfolioViewModel.IsPublished = portfolio.IsPublished;
            var achievementViewModels = new List<AchievementViewModel>();
            if(portfolio.Achievements is not null)
            {
                foreach(var achievement in portfolio.Achievements)
                {
                    var item = new AchievementViewModel();
                    item.AchievementId = achievement.AchievementId;
                    item.AchievementName = achievement.AchievementName;
                    item.Description = achievement.Description;
                    if(achievement.FileModel is not null)
                    {
                        item.FilePath = achievement.FileModel.Path;
                    }
                    else
                    {
                        item.FilePath = null;
                    }
                    item.AchievementTypeName = (await _achievementTypeRepository.GetAsync(achievement.AchievementTypeId)).AchievementTypeName;
                    achievementViewModels.Add(item);
                }
            }
            portfolioViewModel.Achievements = achievementViewModels;
            return portfolioViewModel;
        }
    }
}