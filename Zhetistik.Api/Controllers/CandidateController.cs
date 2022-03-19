using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Zhetistik.Data.ViewModels;

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/candidates")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IAchievementRepository _achievementRepository;
        private readonly UserManager<ZhetistikUser> _userManager;
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<CandidateController> _logger;

        public CandidateController(ICandidateRepository candidateRepository, ISchoolRepository schoolRepository, IFileRepository fileRepository, IPortfolioRepository portfolioRepository, IAchievementRepository achievementRepository, UserManager<ZhetistikUser> userManager, ILocationRepository locationRepository, ILogger<CandidateController> logger)
        {
            _candidateRepository = candidateRepository;
            _schoolRepository = schoolRepository;
            _fileRepository = fileRepository;
            _portfolioRepository = portfolioRepository;
            _achievementRepository = achievementRepository;
            _userManager = userManager;
            _locationRepository = locationRepository;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<CandidateViewModel>> GetCandidateViewModelsAsync()
        {
            var candidates = await _candidateRepository.GetAllAsync();
            var list = new List<CandidateViewModel>();
            foreach(var candidate in candidates)
            {
                var model = await _candidateRepository.GetCandidateViewModelAsync(candidate.CandidateId);
                var portfolio = await _portfolioRepository.GetPortfolioByCandidateAsync(candidate.CandidateId);
                var achievements = await _achievementRepository.GetAchievementsByCandidateAsync(candidate.CandidateId);
                var portfolioViewModel = new PortfolioViewModel();
                portfolioViewModel.PortfolioId = portfolio.PortfolioId;
                portfolioViewModel.IsPublished = portfolio.IsPublished;
                portfolioViewModel.Achievements = achievements;
                model.PortfolioViewModel = portfolioViewModel;
                list.Add(model);
            }
            return list;
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<CandidateViewModel>> GetCandidateAsync(int id)
        {
            var candidateViewModel = await _candidateRepository.GetCandidateViewModelAsync(id);
            if(candidateViewModel is null)
            {
                return NotFound();
            }
            var portfolio = await _portfolioRepository.GetPortfolioByCandidateAsync(candidateViewModel.CandidateId);
            var achievements = await _achievementRepository.GetAchievementsByCandidateAsync(candidateViewModel.CandidateId);
            var portfolioViewModel = new PortfolioViewModel();
            portfolioViewModel.PortfolioId = portfolio.PortfolioId;
            portfolioViewModel.IsPublished = portfolio.IsPublished;
            portfolioViewModel.Achievements = achievements;
            candidateViewModel.PortfolioViewModel = portfolioViewModel;
            return candidateViewModel;
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Candidate>> PostCandidateAsync(CreateCandidateViewModel candidateViewModel)
        {
            var model = new Candidate();
            var user = await _userManager.FindByIdAsync(candidateViewModel.ZhetistikUserId);
            if(user is null)
            {
                return NotFound();
            }
            model.ZhetistikUserId = candidateViewModel.ZhetistikUserId;
            model.Portfolio = new Portfolio();
            await _candidateRepository.CreateAsync(model);
            return CreatedAtAction(nameof(GetCandidateAsync), new { id = model.CandidateId }, model);
        }
        [HttpPut("locations/")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddLocationAsync(int candidateId, int locationId)
        {
            var existingLocation = await _locationRepository.GetAsync(locationId);
            var existingCandidate = await _candidateRepository.GetAsync(candidateId);
            if(existingLocation is null || existingCandidate is null) 
            {
                return NotFound();
            }
            await _candidateRepository.AddLocationAsync(candidateId, existingLocation);
            return StatusCode(StatusCodes.Status204NoContent, new Response{Status = "Success", Message = $"Added location to candidate {candidateId}"});
        }
        [HttpPut("birthdate/")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult> AddBirthdateAsync(int candidateId, int year, int month, int day)
        {
            var birthdate = new DateTime(year, month, day);
            var existingCandidate = await _candidateRepository.GetAsync(candidateId);
            if(existingCandidate is null) 
            {
                return NotFound();
            }
            await _candidateRepository.AddBirthdayAsync(candidateId, birthdate);
            return StatusCode(StatusCodes.Status204NoContent, new Response{Status = "Success", Message = $"Added location to candidate {candidateId}"});
        }
        [HttpPut("school/")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult> AddSchoolAsync(int candidateId, int schoolId, int graduateYear)
        {
            var graduateDate = new DateTime(graduateYear, 5, 25);
            var existingSchool = await _schoolRepository.GetAsync(schoolId);
            var existingCandidate = await _candidateRepository.GetAsync(candidateId);
            if(existingCandidate is null || existingSchool is null) 
            {
                return NotFound();
            }
            await _candidateRepository.AddSchoolAsync(candidateId, existingSchool, graduateDate);
            return StatusCode(StatusCodes.Status204NoContent, new Response{Status = "Success", Message = $"Added location to candidate {candidateId}"});
        }
        [HttpPut("portfolio/")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult> AddAchievementAsync(int candidateId, [FromForm]CreateAchievementViewModel achievementViewModel)
        {
            var existingCandidate = await _candidateRepository.GetAsync(candidateId);
            if(existingCandidate is null) 
            {
                return NotFound();
            }
            var achievement = new Achievement();
            achievement.AchievementName = achievementViewModel.AchievementName;
            achievement.Description = achievementViewModel.Description;
            achievement.AchievementTypeId = achievementViewModel.AchievementTypeId;
            achievement.FileModel = await _fileRepository.SaveFileAsync(achievementViewModel.File);
            achievement.PortfolioId = (await _portfolioRepository.GetPortfolioByCandidateAsync(candidateId)).PortfolioId;
            await _achievementRepository.CreateAsync(achievement);
            return StatusCode(StatusCodes.Status204NoContent, new Response{Status = "Success", Message = $"Added achievement to candidate {candidateId}"});
        }
        [HttpDelete("candidate/")]
        [Authorize(Roles = "Admin, Candidate")]
        public async Task<ActionResult> DeleteCandidateAsync(int id)
        {
            var existingCandidate = await _candidateRepository.GetAsync(id);
            if(existingCandidate is null) 
            {
                return NotFound();
            }
            await _candidateRepository.DeleteAsync(id);
            return StatusCode(StatusCodes.Status204NoContent, new Response{Status = "Success", Message = $"Deleted candidate {id}"});
        }
    }
}