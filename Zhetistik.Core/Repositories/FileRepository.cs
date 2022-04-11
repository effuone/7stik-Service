using System.Data;
using System.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;
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

        public FileRepository(ZhetistikAppContext context, DapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }
        public async Task<bool> DeleteFileAsync(int id)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                string sql = @$"delete from FileModels
                where Id = {id}";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                var result = await command.ExecuteNonQueryAsync();
                return true;
            }
        }
        public async Task<bool> DeleteFilesAsync(int id)
        {
            var model = await GetFileAsync(id);
            string[] files = Directory.GetFiles(model.Path);
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var results = new List<bool>();
                foreach(var file in files)
                {
                    File.Delete(file);
                    var result = await connection.DeleteAsync<FileModel>(new FileModel { Id = (await GetFileAsync(file)).Id });
                    results.Add(result);
                }
                return true;
            }
        }
        public void DeleteDirectory(string path)
        {
            Directory.Delete(path);
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
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var model = await connection.GetAsync<FileModel>(id);
                return model;
            }
        }

        public async Task<IEnumerable<FileModel>> GetFilesAsync()
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var models = (await connection.GetAllAsync<FileModel>()).ToList();
                return models;
            }
        }

        public async Task<int> SaveFileAsync(string env, string path, IFormFile uploadFile)
        {
            // using (var memoryStream = new MemoryStream())
            // {
            //     await uploadFile.CopyToAsync(memoryStream);
            //     // Upload the file if less than 2 MB
            //     if (memoryStream.Length < 2097152)
            //     {
            //         if(!Directory.Exists(env + $"{path}"))
            //         {
            //             Directory.CreateDirectory(path);
            //         }
            //         string fileName = uploadFile.FileName;
            //         var physicalPath = env + @$"{path}\" + fileName;
            //         var file = new FileModel();
            //         file.FileName = fileName;
            //         file.Path = physicalPath;
            //         using(var stream = new FileStream(physicalPath, FileMode.Create))
            //         {
            //             await uploadFile.CopyToAsync(stream);
            //             using(var ms = new MemoryStream())
            //             {
            //                 await uploadFile.CopyToAsync(ms);
            //                 file.Content = ms.ToArray();
            //             }
            //         }
            //         await _context.FileModels.AddAsync(file);
            //         await _context.SaveChangesAsync();
            //         return file;
            //     }
            //     else
            //     {
            //         throw new IndexOutOfRangeException();
            //     }
            // }
            using (var memoryStream = new MemoryStream())
            {
                await uploadFile.CopyToAsync(memoryStream);
                if(memoryStream.Length < 2097152)
                {
                    if(!Directory.Exists(env + $"{path}"))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var physicalPath = env + @$"{path}\" + uploadFile.FileName;
                    var file = new FileModel();
                    file.Content = memoryStream.ToArray();
                    file.FileName = uploadFile.FileName;
                    file.Path = physicalPath;
                    using(var connection = _dapper.CreateConnection())
                    {
                        connection.Open();
                        string sql = @"INSERT INTO FileModels (FileName, Path, Content) VALUES (@fileName, @path, @content) SET @Id = SCOPE_IDENTITY();";
                        var command = new SqlCommand(sql, (SqlConnection)connection);
                        command.Parameters.Add(new SqlParameter("@fileName", file.FileName));
                        command.Parameters.Add(new SqlParameter("@path", file.Path));
                        command.Parameters.Add(new SqlParameter("@content", file.Content));
                        var outputParam = new SqlParameter
                        {
                            ParameterName = "@Id",
                            SqlDbType = SqlDbType.Int,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);
                        await command.ExecuteNonQueryAsync();
                        connection.Close();
                        return (int)outputParam.Value;
                    }
                }
                else
                {
                    throw new OutOfMemoryException();
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