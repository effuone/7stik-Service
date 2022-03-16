using Microsoft.AspNetCore.Authorization;

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/portfolios")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(IPortfolioRepository portfolioRepository, ILogger<PortfolioController> logger)
        {
            _portfolioRepository = portfolioRepository;
            _logger = logger;
        }
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync()
        {
            return await _portfolioRepository.GetAllAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio>> GetPortfolioByAsync(int portfolioId)
        {
            return await _portfolioRepository.GetAsync(portfolioId);
        }
        [Authorize(Roles = "Candidate")]
        [HttpGet("candidate/")]
        public async Task<ActionResult<Portfolio>> GetPortfolioByCandidateAsync(int candidateId)
        {
            return await _portfolioRepository.GetPortfolioByCandidateAsync(candidateId);
        }
    }
}