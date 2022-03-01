using System;
using System.Collections.Generic;

namespace Zhetistik.Data.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        public int CourseLength { get; set; }
    }
}
