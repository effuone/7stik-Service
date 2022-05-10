using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Zhetistik.Data.ViewModels
{
    public class SchoolViewModel
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public DateTime FoundationYear { get; set; }
        public LocationViewModel Location {get; set;}
    }
    public class CreateSchoolViewModel
    {
        [Required]
        public string SchoolName { get; set; }
        public IFormFile Image {get; set;}
        public DateTime FoundationYear {get; set;}
        public int LocationId { get; set; }
    }
    public class UpdateCandidateAsync
    {
        [Required]
        public string SchoolName { get; set; }
        public IFormFile Image {get; set;}
        public DateTime FoundationYear {get; set;}
        public int LocationId { get; set; }
    }
}