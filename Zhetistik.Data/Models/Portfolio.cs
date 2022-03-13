using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Portfolios")]
    public class Portfolio
    {
        [Key]
        public int PortfolioId { get; set; }
        public int CandidateId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public bool IsPublished { get; set; }
        [JsonIgnore]
        public Candidate Candidate { get; set; }
        public IEnumerable<Achievement> Achievements {get; set;}
    }
}
