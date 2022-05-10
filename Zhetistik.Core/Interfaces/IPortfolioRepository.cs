namespace Zhetistik.Core.Interfaces
{
    public interface IPortfolioRepository : IAsyncRepository<Portfolio>
    {
        public Task<Portfolio> GetPortfolioByCandidateAsync(int candidateId);
        public Task<int> CreatePortfolioWithCandidateAsync(int candidateId);
    }
}