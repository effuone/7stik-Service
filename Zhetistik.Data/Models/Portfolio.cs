using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zhetistik.Data.Models
{
    public class Portfolio
    {
        public int PortofolioId { get; set; }
        public int CandidateId { get; set; }
        public int LocationId { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public Candidate Candidate { get; set; }
        [JsonIgnore]
        public Location Location { get; set; }
    }
}
