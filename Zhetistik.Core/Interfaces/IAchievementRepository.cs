using Zhetistik.Data.ViewModels;

namespace Zhetistik.Core.Interfaces
{
    public interface IAchievementRepository : IAsyncRepository<Achievement>
    {
        public Task<IEnumerable<Achievement>> GetAchievementsByPortfolioAsync(int portfolioId);
        public Task<IEnumerable<AchievementViewModel>> GetAchievementsByCandidateAsync(int candidateId);
        public Task<IEnumerable<AchievementViewModel>> GetAllAchievementViewModelsAsync();
    }
}