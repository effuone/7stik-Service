namespace Zhetistik.Data.ViewModels
{
    public class CandidateViewModel
    {
        public int CandidateId { get; set; }
        public int PortfolioId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public SchoolViewModel School { get; set; }
        public LocationViewModel Location { get; set; }
        public DateTime GraduateYear { get; set; }
    }
    public class CreateCandidateViewModel
    {
        public string ZhetistikUserId { get; set; }
    }
    public class UpdateCandidateViewModel
    {
        public string ZhetistikUserId { get; set; }
    }
}