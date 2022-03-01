using System;
using System.Collections.Generic;

namespace Zhetistik.Data.Models
{
    public class Candidate
    {

        public int CandidateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Portfolio Portfolio { get; set; }
    }
}
