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
        void DeleteDirectory(string path);
        Task<FileModel> GetFileAsync(string fileName);
        Task<bool> DeleteFilesAsync(int id);
        Task<IEnumerable<FileModel>> GetFilesAsync();
        Task<int> SaveFileAsync(string env, string path, IFormFile uploadFile);
        Task<bool> UpdateFileAsync(int fileId, IFormFile uploadFile);
        Task<bool> DeleteFileAsync(int id);
    }
}