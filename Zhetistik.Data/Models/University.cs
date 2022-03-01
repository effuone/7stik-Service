using System;
using System.Collections.Generic;

namespace Zhetistik.Data.Models
{
    public class University
    {
        public int UniversityId { get; set; }
        public string UniversityName { get; set; }
        public string UniversityDescription { get; set; }
        public int LocationId { get; set; }
        public DateTime FoundationYear { get; set; }
        public int StudentsCount { get; set; }
        public List<UniversityType> UniversityTypes { get; set; }
    }
}
