using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Mentors")]
    public class Mentor
    {
        [Key]
        public int MentorId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string FirstName { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string LastName { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Email { get; set; }
        public string CellphoneNumber { get; set; }
        public Company Company { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("CompanyId")]
        public int CompanyId { get; set; }
    }
}