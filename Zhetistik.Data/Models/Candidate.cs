using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Candidates")]
    public class Candidate
    {
        [Key]
        public int CandidateId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string ZhetistikUserId { get; set; }
        public DateTime? Birthday { get; set; }
        [JsonIgnore]
        public Location Location { get; set; }
        [JsonIgnore]
        public School School {get; set;}
        public DateTime? GraduateYear {get; set;}
        public Portfolio Portfolio { get; set;}
    }
}
