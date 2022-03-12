// using Microsoft.AspNetCore.StaticFiles;
// using Zhetistik.Data.ViewModels;

// namespace Zhetistik.Api.Controllers
// {
//     [ApiController]
//     [Route("api/achievements")]
//     public class AchievementController : ControllerBase
//     {
//         private readonly IAchievementRepository _achievementRepository;
//         private readonly IFileRepository _fileRepository;
//         private readonly IWebHostEnvironment _env;
//         private readonly ILogger<AchievementController> _logger;

//         public AchievementController(IAchievementRepository achievementRepository, IFileRepository fileRepository, IWebHostEnvironment env, ILogger<AchievementController> logger)
//         {
//             _achievementRepository = achievementRepository;
//             _fileRepository = fileRepository;
//             _env = env;
//             _logger = logger;
//         }

//         [HttpGet]
//         public async Task<IEnumerable<Achievement>> GetAchievementsAsync()
//         {
//             return await _achievementRepository.GetAllAsync();
//         }
//         [HttpGet("{id}")]
//         public async Task<ActionResult<Achievement>> GetAchievementAsync(int id)
//         {
//             var existingAchievement = await _achievementRepository.GetAsync(id);
//             if(existingAchievement is null)
//             {
//                 return NotFound();
//             }
//             return existingAchievement;
//         }
//         [HttpPost]
//         public async Task<ActionResult<Achievement>> AddAchievementAsync(int portfolioId, CreateAchievementViewModel achievementViewModel)
//         {
//             var fileId = await _fileRepository.SaveFileAsync(achievementViewModel.File);
//             // var achievement = new Achievement();
//             // achievement.AchievementName = achievementViewModel.AchievementName;
//             // achievement.AchievementTypeId = achievementViewModel.AchievementTypeId;
//             // if (achievementViewModel.File != null)
//             // {
//             //     var uploadedFile = achievementViewModel.File;
//             //     // путь к папке Files
//             //     string path = "/AchievementFiles/" + uploadedFile.FileName;
//             //     // сохраняем файл в папку Files в каталоге wwwroot
//             //     using (var fileStream = new FileStream(_env.WebRootPath + path, FileMode.Create))
//             //     {
//             //         await uploadedFile.CopyToAsync(fileStream);
//             //     }
//             //     FileModel file = new FileModel { FileName = uploadedFile.FileName, FilePath = path};
//             // }
//             // achievement.PortfolioId = portfolioId;
//             // await _achievementRepository.CreateAsync(achievement);
//             // return CreatedAtAction(nameof(GetAchievementAsync), new { id = achievement.AchievementId }, achievement);
//         }
//         [HttpGet("files/{id}")]
//         public async Task<ActionResult> DownloadFile(int id)
//         {
//             var provider = new FileExtensionContentTypeProvider();
//             if (!provider.TryGetContentType(filePath, out var contentType))
//             {
//                 contentType = "application/octet-stream";
//             }
            
//             var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
//             return File(bytes, contentType, Path.GetFileName(filePath));
//         }
//     }
// }