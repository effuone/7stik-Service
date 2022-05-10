using Zhetistik.Data.ViewModels;

namespace Zhetistik.Core.Interfaces
{
    public interface ICandidateRepository : IAsyncRepository<Candidate>
    {
        public Task<int> CreateAsync(string zhetistikUserId);
        public Task<Candidate> GetAsync(string userId);
        public Task<Candidate> GetByPortfolioAsync(int portfolioId);
        public Task<CandidateViewModel> GetCandidateViewModelAsync(int candidateId);
        public Task AddBirthdayAsync(int candidateId, DateTime birthday);
        public Task AddLocationAsync(int candidateId, Location location);
        public Task AddSchoolAsync(int candidateId, School school, DateTime graduateDate);
    }
}