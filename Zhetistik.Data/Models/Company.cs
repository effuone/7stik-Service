using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string CompanyName { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Mentor> Mentors { get; set; }
    }
}