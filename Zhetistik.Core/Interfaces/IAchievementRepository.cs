namespace Zhetistik.Core.Interfaces
{
    public interface IAchievementRepository : IAsyncRepository<Achievement>
    {
        public Task<IEnumerable<Achievement>> GetAchievementsByPortfolioAsync(int portfolioId);
    }
}