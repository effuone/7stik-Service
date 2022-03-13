global using Dapper;
using Zhetistik.Core.DataAccess;
using Zhetistik.Core.Interfaces;
using Zhetistik.Data.Context;

namespace Zhetistik.Core.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ZhetistikAppContext _context;
        private readonly DapperContext _dapper;

        public CandidateRepository(ZhetistikAppContext context, DapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task AddBirthdayAsync(int candidateId, DateTime birthday)
        {
            using(var connection = _dapper.CreateConnection())
            {
                string updateQuery = @"UPDATE [dbo].[Candidates] SET Birthday = @birthday WHERE CandidateId = @candidateId";
                var result = await connection.ExecuteAsync(updateQuery, new
                {
                    birthday,
                    candidateId
                });
            }
        }

        public async Task AddLocationAsync(int candidateId, Location location)
        {
            using(var connection = _dapper.CreateConnection())
            {
                string updateQuery = @"UPDATE [dbo].[Candidates] SET LocationId = @locationId WHERE CandidateId = @candidateId";
                var result = await connection.ExecuteAsync(updateQuery, new
                {
                    location.LocationId,
                    candidateId
                });
            }
        }

        public async Task AddSchoolAsync(int candidateId, School school, DateTime graduateDate)
        {
            using(var connection = _dapper.CreateConnection())
            {
                string updateQuery = @"UPDATE [dbo].[Candidates] SET SchoolId = @schoolId, GraduateYear = @graduateDate WHERE CandidateId = @candidateId";
                var result = await connection.ExecuteAsync(updateQuery, new
                {
                    school.SchoolId,
                    graduateDate,
                    candidateId
                });
            }
        }

        public async Task<int> CreateAsync(Candidate model)
        {
            await _context.Candidates.AddAsync(model);
            await _context.SaveChangesAsync();
            return model.CandidateId;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var model = await _context.Candidates.FindAsync(id);
            var result = _context.Candidates.Remove(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Candidate>> GetAllAsync()
        {
            return await _context.Candidates.ToListAsync();
        }

        public async Task<Candidate> GetAsync(int id)
        {
            return await _context.Candidates.FindAsync(id);
        }

        public async Task<Candidate> GetAsync(string userId)
        {
            return await _context.Candidates.FirstOrDefaultAsync(x=>x.ZhetistikUserId == userId);
        }

        public async Task<Candidate> GetByPortfolioAsync(int portfolioId)
        {
            var existingPortfolio = await _context.Portfolios.FindAsync(portfolioId);
            return await _context.Candidates.FindAsync(existingPortfolio.CandidateId);
        }

        public async Task<bool> UpdateAsync(int id, Candidate model)
        {
            var existingModel = await _context.Candidates.FindAsync(id);
            model.CandidateId = id;
            _context.Candidates.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
