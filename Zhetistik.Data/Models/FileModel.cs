using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("FileModels")]
    public class FileModel
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        [JsonIgnore]
        public byte[] Content { get; set; }
    }
}