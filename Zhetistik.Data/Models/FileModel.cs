using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Files")]
    public class FileModel
    {
        public int FileId { get; set; }
        public byte[] Content { get; set; }
    }
}