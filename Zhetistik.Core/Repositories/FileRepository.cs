using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Zhetistik.Core.Interfaces;
using Zhetistik.Data.Context;

namespace Zhetistik.Core.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly ZhetistikAppContext _context;

        public FileRepository(ZhetistikAppContext context)
        {
            _context = context;
        }

        public Task<bool> DeleteFileAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<FileModel> GetFileAsync(int id)
        {
            return await _context.Files.FindAsync(id);
        }

        public async Task<IEnumerable<FileModel>> GetFilesAsync()
        {
            return await _context.Files.ToListAsync();
        }

        public async Task<Tuple<int, bool>> SaveFileAsync(IFormFile uploadFile)
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

                    await _context.Files.AddAsync(file);
                    await _context.SaveChangesAsync();
                    return new Tuple<int, bool>(file.FileId, true);
                }
                else
                {
                    return new Tuple<int, bool>(0, false);
                }
            }
        }

        public Task<bool> UpdateFileAsync(int fileId, IFormFile uploadFile)
        {
            throw new NotImplementedException();
        }

        Task<int> IFileRepository.SaveFileAsync(IFormFile uploadFile)
        {
            throw new NotImplementedException();
        }
    }
}