using Microsoft.AspNetCore.Http;

namespace Zhetistik.Core.Interfaces
{
    public interface IFileRepository
    {
        //https://localhost:7259/api/portfolios?id=1824/achievements/
        //POST AddAchievementAsync (AchievementViewModel)
        //AchievementViewModel == 
        Task<FileModel> GetFileByAchievementAsync(int achievementId);
        Task<FileModel> GetFileAsync(int id);
        Task<IEnumerable<FileModel>> GetFilesAsync();
        Task<FileModel> SaveFileAsync(IFormFile uploadFile);
        Task<bool> UpdateFileAsync(int fileId, IFormFile uploadFile);
        Task<bool> DeleteFileAsync(int id);
    }
}