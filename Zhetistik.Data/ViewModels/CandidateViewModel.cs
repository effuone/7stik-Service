using Zhetistik.Data.Models;
namespace Zhetistik.Data.ViewModels
{
    public class CandidateViewModel
    {
        public int CandidateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? GraduateYear { get; set; }
        #nullable enable
        public SchoolViewModel? School { get; set; }
        public LocationViewModel? Location {get; set;}
        public PortfolioViewModel? PortfolioViewModel {get; set;}
        #nullable disable

    }
    public class CreateCandidateViewModel
    {   
        public string ZhetistikUserId { get; set; }
    }
}