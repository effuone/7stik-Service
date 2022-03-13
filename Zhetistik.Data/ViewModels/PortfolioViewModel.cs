namespace Zhetistik.Data.ViewModels
{
    public class PortfolioViewModel
    {
        public int PortfolioId { get; set; }
        public bool IsPublished { get; set; }
        public IEnumerable<AchievementViewModel> Achievements {get; set;}
    }
    public class CreatePortfolioViewModel
    {
        public CandidateViewModel CandidateViewModel { get; set; }
        public IEnumerable<AchievementViewModel> AchievementViewModels { get; set; }
        public bool IsPublishing { get; set; }
    }
}