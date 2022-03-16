using System.Data.SqlClient;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Zhetistik.Core.DataAccess;
using Zhetistik.Core.Interfaces;
using Zhetistik.Data.Context;

namespace Zhetistik.Core.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly ZhetistikAppContext _context;
        private readonly DapperContext _dapper;
        private const string _achievementFolder = "/AchievementFiles";
        private const string _candidateFolder = "/CandidatePhotos";
        private const string _schoolFolder = "/SchoolPhotos";

        public FileRepository(ZhetistikAppContext context, DapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task<bool> DeleteFileAsync(int id)
        {
            var model = await _context.FileModels.FindAsync(id);
            _context.Remove(model);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<FileModel> GetFileByAchievementAsync(int achievementId)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var fileModel = await connection.QueryFirstAsync<FileModel>(@$"select fm.Id, fm.Content from FileModels as fm, Achievements as ac
                where ac.FileModelId = fm.Id and ac.AchievementId = {achievementId}");
                return fileModel;
            }
        }
        public async Task<FileModel> GetFileAsync(int id)
        {
            return await _context.FileModels.FindAsync(id);
        }

        public async Task<IEnumerable<FileModel>> GetFilesAsync()
        {
            return await _context.FileModels.ToListAsync();
        }

        public async Task<FileModel> SaveFileAsync(IFormFile uploadFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await uploadFile.CopyToAsync(memoryStream);

                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    var file = new FileModel()
                    {
                        Content = memoryStream.ToArray()
                    };

                    await _context.FileModels.AddAsync(file);
                    await _context.SaveChangesAsync();
                    return file;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public Task<bool> UpdateFileAsync(int fileId, IFormFile uploadFile)
        {
            throw new NotImplementedException();
        }
    }
}