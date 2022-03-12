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
        private readonly ILogger<PortfolioController> _logger;
        private readonly IWebHostEnvironment _env;

        public PortfolioController(IPortfolioRepository portfolioRepository, IAchievementRepository achievementRepository, ILogger<PortfolioController> logger, IWebHostEnvironment env)
        {
            _portfolioRepository = portfolioRepository;
            _achievementRepository = achievementRepository;
            _logger = logger;
            _env = env;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync()
        {
            return await _portfolioRepository.GetAllAsync();
        }
    }
}