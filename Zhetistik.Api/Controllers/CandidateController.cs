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
        public async Task<IEnumerable<Candidate>> GetCandidatesAsync()
        {
            var candidates = await _candidateRepository.GetAllAsync();
            foreach(var candidate in candidates)
            {
                candidate.Portfolio = await _portfolioRepository.GetPortfolioByCandidateAsync(candidate.CandidateId);
                var achievements = await _achievementRepository.GetAchievementsByPortfolioAsync(candidate.Portfolio.PortfolioId);
                foreach(var achievement in achievements)
                {
                    achievement.FileModel = await _fileRepository.GetFileByAchievementAsync(achievement.AchievementId);
                }
                candidate.Portfolio.Achievements = achievements;
            }
            return candidates;
        }
        [HttpGet("vm")]
        public async Task<IEnumerable<CandidateViewModel>> GetCandidateViewModelsAsync()
        {
            var list = new List<CandidateViewModel>();
            var candidates =  await _candidateRepository.GetAllAsync();
            foreach(var existingModel in candidates)
            {
                var user = await _userManager.FindByIdAsync(existingModel.ZhetistikUserId);
                var candidateViewModel = new CandidateViewModel();
                var schoolViewModel = new SchoolViewModel();
                var schoolLocationViewModel = new LocationViewModel();
                var candidateLocationViewModel = new LocationViewModel();

                schoolLocationViewModel.CityName = existingModel.School.Location.City.CityName;
                schoolLocationViewModel.CountryName = existingModel.School.Location.Country.CountryName;
                schoolLocationViewModel.LocationId = existingModel.School.LocationId;

                candidateLocationViewModel.CityName = existingModel.Location.City.CityName;
                candidateLocationViewModel.CountryName = existingModel.Location.Country.CountryName;
                candidateLocationViewModel.LocationId = existingModel.Location.LocationId;

                schoolViewModel.FoundationYear = existingModel.School.FoundationYear;
                schoolViewModel.SchoolName = existingModel.School.SchoolName;
                schoolViewModel.Location = schoolLocationViewModel;

                candidateViewModel.Birthday = (DateTime)existingModel.Birthday;
                candidateViewModel.FirstName = user.FirstName;
                candidateViewModel.LastName = user.LastName;
                candidateViewModel.GraduateYear = (DateTime)existingModel.GraduateYear;
                candidateViewModel.School = schoolViewModel;
                candidateViewModel.Location = candidateLocationViewModel;

                var portfolioViewModel = new PortfolioViewModel();
                var existingPortfolio = await _portfolioRepository.GetPortfolioByCandidateAsync(existingModel.CandidateId);
                foreach(var achievement in existingPortfolio.Achievements)
                {
                    var achievementViewModel = new AchievementViewModel();
                    achievementViewModel.AchievementName = achievement.AchievementName;
                    achievementViewModel.AchievementTypeId = achievement.AchievementTypeId;
                    achievementViewModel.FileId = achievement.FileModel.Id;
                    achievementViewModel.Description = achievement.Description;
                    portfolioViewModel.Achievements.Append(achievementViewModel);
                }
                portfolioViewModel.IsPublished = existingPortfolio.IsPublished;
                portfolioViewModel.PortfolioId = existingPortfolio.PortfolioId;
                candidateViewModel.PortfolioViewModel = portfolioViewModel;
                list.Add(candidateViewModel); 
            }
            return list;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CandidateViewModel>> GetCandidateAsync(int id)
        {
            var existingModel = await _candidateRepository.GetAsync(id);
            if(existingModel is null)
            {
                return NotFound();
            }
            //Init
            var user = await _userManager.FindByIdAsync(existingModel.ZhetistikUserId);
            var candidateViewModel = new CandidateViewModel();
            var schoolViewModel = new SchoolViewModel();
            var schoolLocationViewModel = new LocationViewModel();
            var candidateLocationViewModel = new LocationViewModel();

            schoolLocationViewModel.CityName = existingModel.School.Location.City.CityName;
            schoolLocationViewModel.CountryName = existingModel.School.Location.Country.CountryName;
            schoolLocationViewModel.LocationId = existingModel.School.LocationId;

            candidateLocationViewModel.CityName = existingModel.Location.City.CityName;
            candidateLocationViewModel.CountryName = existingModel.Location.Country.CountryName;
            candidateLocationViewModel.LocationId = existingModel.Location.LocationId;

            schoolViewModel.FoundationYear = existingModel.School.FoundationYear;
            schoolViewModel.SchoolName = existingModel.School.SchoolName;
            schoolViewModel.Location = schoolLocationViewModel;

            candidateViewModel.Birthday = (DateTime)existingModel.Birthday;
            candidateViewModel.FirstName = user.FirstName;
            candidateViewModel.LastName = user.LastName;
            candidateViewModel.GraduateYear = (DateTime)existingModel.GraduateYear;
            candidateViewModel.School = schoolViewModel;
            candidateViewModel.Location = candidateLocationViewModel;
            return candidateViewModel;
        }
        [HttpPost]
        public async Task<ActionResult<Candidate>> PostCandidateAsync(CreateCandidateViewModel candidateViewModel)
        {
            var model = new Candidate();
            var user = await _userManager.FindByIdAsync(candidateViewModel.ZhetistikUserId);
            if(user is null)
            {
                return NotFound();
            }
            // var existingCandidate = _candidateRepository.GetAsync(candidateViewModel.ZhetistikUserId);
            // if(existingCandidate is not null)
            // {
            //     return StatusCode(StatusCodes.Status409Conflict, new Response { Status = "Error", Message = "Candidate creation failed! Candidate already exists" });  
            // }
            model.ZhetistikUserId = candidateViewModel.ZhetistikUserId;
            model.Portfolio = new Portfolio();
            await _candidateRepository.CreateAsync(model);
            return CreatedAtAction(nameof(GetCandidateAsync), new { id = model.CandidateId }, model);
        }
        [HttpPut("locations/")]
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
    }
}