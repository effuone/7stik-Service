using System.Data.SqlClient;
using System.Net;
using System.Net.Http.Headers;
using Dapper;
using Microsoft.AspNetCore.Http;
using Zhetistik.Core.DataAccess;
using Zhetistik.Core.Interfaces;
using Zhetistik.Data.Context;

namespace Zhetistik.Api.Controllers
{
    public class FileRepository : IFileRepository
    {
        private readonly ZhetistikAppContext _context;
        private readonly DapperContext _dapper;
        private readonly IWebHostEnvironment _env;

        public FileRepository(ZhetistikAppContext context, DapperContext dapper, IWebHostEnvironment env)
        {
            _context = context;
            _dapper = dapper;
            _env = env;
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
                var fileModel = await connection.QueryFirstAsync<FileModel>(@$"select fm.Id, fm.FileName,fm.Path from FileModels as fm, Achievements as ac
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

        public async Task<FileModel> SaveFileAsync(string path, IFormFile uploadFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await uploadFile.CopyToAsync(memoryStream);
                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    string fileName = uploadFile.FileName;
                        var physicalPath = _env.ContentRootPath + $"/{path}/" + fileName;
                        var file = new FileModel();
                        file.FileName = fileName;
                        file.Path = _env.ContentRootPath + @$"\{path}\" + fileName;
                        using(var stream = new FileStream(physicalPath, FileMode.Create))
                        {
                            await uploadFile.CopyToAsync(stream);
                            using(var ms = new MemoryStream())
                            {
                                await uploadFile.CopyToAsync(ms);
                                file.Content = ms.ToArray();
                            }
                        }
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


        public async Task<bool> UpdateFileAsync(int fileId, IFormFile uploadFile)
        {
            var existingFile = await _context.FileModels.FindAsync(fileId);
            using(var ms = new MemoryStream())
            {
                await uploadFile.CopyToAsync(ms);
                if(ms.Length < 2097152)
                {
                    existingFile.Content = ms.ToArray();
                    existingFile.FileName = uploadFile.FileName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<FileModel> GetFileAsync(string fileName)
        {
            var list = _context.FileModels.Where(x=>x.FileName == fileName);
            var file = await list.FirstOrDefaultAsync();
            return file;
        }
    }
}