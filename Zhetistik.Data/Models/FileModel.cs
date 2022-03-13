using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Files")]
    public class FileModel
    {
        [Key]
        public int Id { get; set; }
        public byte[] Content { get; set; }
    }
}