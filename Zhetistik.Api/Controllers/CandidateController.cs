using Microsoft.AspNetCore.Identity;
using Zhetistik.Data.ViewModels;

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/candidates/")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ILogger<CandidateController> _logger;
        private readonly UserManager<ZhetistikUser> _userManager;

        public CandidateController(ICandidateRepository candidateRepository, ISchoolRepository schoolRepository, ILocationRepository locationRepository, IPortfolioRepository portfolioRepository, ILogger<CandidateController> logger, UserManager<ZhetistikUser> userManager)
        {
            _candidateRepository = candidateRepository;
            _schoolRepository = schoolRepository;
            _locationRepository = locationRepository;
            _portfolioRepository = portfolioRepository;
            _logger = logger;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IEnumerable<Candidate>> GetCandidatesAsync()
        {
            return await _candidateRepository.GetAllAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Candidate>> GetCandidateAsync(int id)
        {
            var existingModel = await _candidateRepository.GetAsync(id);
            if(existingModel is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response{Status="Error", Message="Candidate not found"});
            }
            return existingModel;
        }
        [HttpGet]
        [Route("vm")]
        public async Task<ActionResult<CandidateViewModel>> GetCandidateViewModelAsync(int id)
        {
            var existingModel = await _candidateRepository.GetCandidateViewModelAsync(id);
            if(existingModel is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response{Status="Error", Message="Candidate not found"});
            }
            return existingModel;
        }
        [HttpPost]
        public async Task<ActionResult> PostCandidateAsync([FromRoute]string zhetistikUserId)
        {
            var existingModel = await _candidateRepository.GetAsync(zhetistikUserId);
            if(existingModel is not null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response{Status="Error", Message="Candidate already exists"});
            }
            var user = await _userManager.FindByIdAsync(zhetistikUserId);
            var id = await _candidateRepository.CreateAsync(zhetistikUserId);
            return StatusCode(StatusCodes.Status200OK, new Response{Status="Success", Message=$"Candidate {user.FirstName} {user.LastName} of id:{id} created"});
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteCandidateAsync(int id)
        {
            var existingModel = await _candidateRepository.GetAsync(id);
            if(existingModel is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response{Status="Error", Message="Candidate not found"});
            }
            await _candidateRepository.DeleteAsync(id);
            return StatusCode(StatusCodes.Status200OK, new Response{Status="Success", Message=$"Candidate of id:{id} deleted"});
        }
    }
}