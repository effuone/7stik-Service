using Zhetistik.Data.Models;
namespace Zhetistik.Data.ViewModels
{
    public class CandidateViewModel
    {
        public int CandidateId { get; set; }
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public DateTime Birthday {get; set;}
        #nullable enable
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public string? SchoolName { get; set; }
        public DateTime? GraduateYear { get; set; }
        #nullable disable
    }
    public class CreateCandidateViewModel
    {   
        public string ZhetistikUserId { get; set; }
    }
}