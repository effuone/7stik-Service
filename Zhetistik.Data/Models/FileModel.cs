using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Files")]
    public class FileModel
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public byte[] Content { get; set; }
    }
}