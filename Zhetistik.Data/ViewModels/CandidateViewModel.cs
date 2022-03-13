using Zhetistik.Data.Models;

namespace Zhetistik.Data.ViewModels
{
    public class CandidateViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime GraduateYear { get; set; }
        public SchoolViewModel School { get; set; }
        public LocationViewModel Location {get; set;}
        public PortfolioViewModel PortfolioViewModel {get; set;}

    }
    public class CreateCandidateViewModel
    {   
        public string ZhetistikUserId { get; set; }
    }
    public class UpdateCandidateViewModel
    {
        
    }
}