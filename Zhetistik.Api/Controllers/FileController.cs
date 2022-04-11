namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly ZhetistikAppContext _context;
        private readonly DapperContext _dapper;
        private readonly IFileRepository _fileRepository;
        private readonly IWebHostEnvironment _env;
        private const string _achievementFolder = "/AchievementFiles";
        private const string _candidateFolder = "/CandidatePhotos";
        private const string _schoolFolder = "/SchoolPhotos";

        public FileController(ZhetistikAppContext context, DapperContext dapper, IFileRepository fileRepository, IWebHostEnvironment env)
        {
            _context = context;
            _dapper = dapper;
            _fileRepository = fileRepository;
            _env = env;
        }
        private bool PostDirectoryAsync(string path, string directoryName)
        {
            if(Directory.Exists(path  + '/' + directoryName))
            {
                return false;
            }
            Directory.CreateDirectory(path + '/' + directoryName);
            return true;
        }
        [HttpPost]
        public async Task<ActionResult<FileModel>> PostFileAsync(IFormFile uploadFile)
        {
            var id = await _fileRepository.SaveFileAsync(_env.ContentRootPath, "Files", uploadFile);
            return CreatedAtAction(nameof(GetFileAsync), new { id = id }, id);
        }
        [HttpGet]
        public async Task<IEnumerable<FileModel>> GetAllFilesAsync()
        {
            return await _fileRepository.GetFilesAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<FileModel>> GetFileAsync(int id)
        {
            var existingFile =await _fileRepository.GetFileAsync(id);
            if(existingFile is null)
            {
                return NotFound();
            }
            return existingFile;
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFileAsync(int id)
        {
            var existingFile =await _fileRepository.GetFileAsync(id);
            if(existingFile is null)
            {
                return NotFound();
            }
            await _fileRepository.DeleteFileAsync(id);
            return StatusCode(StatusCodes.Status204NoContent, new Response { Status = "Success", Message = $"Deleted file of id {id}"});
        }
    }
}