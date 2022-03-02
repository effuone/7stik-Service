using System;
using System.Collections.Generic;

namespace Zhetistik.Data.Models
{
    public class Candidate
    {
        public int CandidateId { get; set; }
        public Guid ZhetistikUserId { get; set; }
        public DateTime Birthday { get; set; }
        public Portfolio Portfolio { get; set; }
    }
}
