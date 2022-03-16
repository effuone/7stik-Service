namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileRepository fileRepository, ILogger<FileController> logger)
        {
            _fileRepository = fileRepository;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IEnumerable<FileModel>> GetFileModelsAsync()
        {
            return await _fileRepository.GetFilesAsync();
        }
        [HttpPost]
        public async Task<ActionResult<FileModel>> PostFileModelAsync(IFormFile uploadFile)
        {
            var model = await _fileRepository.SaveFileAsync(uploadFile);
            return model;
        }
        [Route("achievement")]
        [HttpGet]
        public async Task<ActionResult<FileModel>> GetFileByAchievementAsync(int id)
        {
            var fileModel = await _fileRepository.GetFileByAchievementAsync(id);
            return fileModel;
        }
    }
}