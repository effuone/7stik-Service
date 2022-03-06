using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zhetistik.Data.Models
{
    public class Portfolio
    {
        public int PortfolioId { get; set; }
        public int CandidateId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public bool IsPublished { get; set; }
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public Candidate Candidate { get; set; }
        public IEnumerable<Achievement> Achievements {get; set;}
    }
}
