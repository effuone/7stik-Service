using System.ComponentModel.DataAnnotations;

namespace Zhetistik.Data.ViewModels
{
    public class PortfolioViewModel
    {
        public int PortfolioId { get; set; }
        public int CandidateId { get; set; }
        public bool IsPublished { get; set; }
        public IEnumerable<AchievementViewModel> Achievements {get; set;}
    }
    public class CreatePortfolioViewModel
    {
        [Required]
        public int CandidateId { get; set; }
    }
    public class UpdatePortfolioViewModel
    {
        public int CandidateId { get; set; }
        public int PortfolioId { get; set; }
        public bool IsPublished { get; set; }
        public IEnumerable<CreateAchievementViewModel> Achievements {get; set;}
    }
}