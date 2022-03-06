using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Schools")]
    public class School
    {
        [Key]
        public int SchoolId { get; set; }
        public int LocationId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string SchoolName { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Image {get; set;}
        public DateTime FoundationYear { get; set; }
        public Location Location { get; set; }
        public IEnumerable<Candidate> Candidates {get; set;}
    }
}
