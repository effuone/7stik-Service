using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Zhetistik.Data.Models
{
    [Table("News")]
    public class News
    {
        public int NewsId { get; set; }
        [JsonIgnore]
        public Editor Editor { get; set; }
        public int EditorId { get; set; }
    }
}