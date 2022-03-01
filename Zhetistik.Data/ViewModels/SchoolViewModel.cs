using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Zhetistik.Data.ViewModels
{
    public class CreateSchoolViewModel
    {
        [Required]
        public string SchoolName { get; set; }
        public string Image {get; set;}
        public DateTime FoundationYear {get; set;}
        public int LocationId { get; set; }
    }
    public class UpdateSchoolViewModel
    {
        [Required]
        public string SchoolName { get; set; }
        public string Image {get; set;}
        public DateTime FoundationYear {get; set;}
        public int LocationId { get; set; }
    }
}