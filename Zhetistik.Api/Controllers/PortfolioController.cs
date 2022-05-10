using Zhetistik.Data.ViewModels;

namespace Zhetistik.Api.Controllers
{
    public class PortfolioController : ControllerBase
    {
        //Get portfolio
        //Get portfolios
        //Create portfolio
        //Update portfolio (example: make it visible for others)
        //Delete portfolio 
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IAchievementRepository _achievementRepository;

        public PortfolioController(IPortfolioRepository portfolioRepository, IAchievementRepository achievementRepository, ILogger<PortfolioController> logger)
        {
            _portfolioRepository = portfolioRepository;
            _achievementRepository = achievementRepository;
            _logger = logger;
        }
        private readonly ILogger<PortfolioController> _logger;
        [HttpGet]
        public async Task<IEnumerable<Portfolio>> GetPortfolioViewModelsAsync()
        {
            return await _portfolioRepository.GetAllAsync();
        }
        [HttpGet("vm")]
        public async Task<ActionResult<PortfolioViewModel>> GetPortfolioViewModelAsync(int id)
        {
            var model = await _portfolioRepository.GetAsync(id);
            if(model is null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var viewModel = new PortfolioViewModel();
            viewModel.CandidateId = model.CandidateId; 
            viewModel.PortfolioId = model.PortfolioId;
            viewModel.IsPublished = model.IsPublished;
            viewModel.Achievements = await _achievementRepository.GetAchievementsByPortfolioAsync(id);
            return viewModel;
        }
        [HttpPost]
        public async Task<ActionResult> CreatePortfolioAsync(CreatePortfolioViewModel createPortfolioViewModel)
        {
            var model = new Portfolio();
            model.CandidateId = createPortfolioViewModel.CandidateId;
            var id = await _portfolioRepository.CreateAsync(model);
            return CreatedAtAction(nameof(GetPortfolioViewModelAsync), new {CandidateId = createPortfolioViewModel.CandidateId, PortfolioId = model.PortfolioId} );
        }
    }
}