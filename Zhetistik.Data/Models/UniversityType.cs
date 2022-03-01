using System;
using System.Collections.Generic;

namespace Zhetistik.Data.Models
{
    public class UniversityType
    {
        public int UniversityTypeId { get; set; }
        public string UniversityTypeName { get; set; }
        public List<University> Universities { get; set; }
    }
}
