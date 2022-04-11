global using Dapper;
using System.Data.SqlClient;
using Zhetistik.Core.DataAccess;
using Zhetistik.Core.Interfaces;
using Zhetistik.Data.Context;
using Zhetistik.Data.ViewModels;

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

        public async Task<CandidateViewModel> GetCandidateViewModelAsync(int candidateId)
        {
            using(var connection = _dapper.CreateConnection())
            {
                var candidateViewModel = new CandidateViewModel();
                connection.Open();
                {
                    string sql = 
                                @"exec GetCandidates";
                    var command = new SqlCommand(sql, (SqlConnection)connection);
                    var reader = await command.ExecuteReaderAsync();
                    if(reader.HasRows is false)
                    {
                        return null;
                    }
                    await reader.ReadAsync();
                    candidateViewModel.CandidateId = candidateId;
                    candidateViewModel.FirstName = reader.GetString(2);
                    candidateViewModel.LastName = reader.GetString(3);
                    candidateViewModel.Birthday = Convert.IsDBNull(reader.GetDateTime(3)) is false ? reader.GetDateTime(3) : DateTime.MinValue;
                    candidateViewModel.CountryName = Convert.IsDBNull(reader.GetString(5)) is false ? reader.GetString(5) : null;
                    candidateViewModel.CityName = Convert.IsDBNull(reader.GetString(6)) is false ? reader.GetString(6) : null;
                    candidateViewModel.SchoolName = Convert.IsDBNull(reader.GetString(7)) is false ? reader.GetString(7) : null;
                    candidateViewModel.GraduateYear = Convert.IsDBNull(reader.GetDateTime(8)) is false ? reader.GetDateTime(8) : null;
                    connection.Close();
                }
                return candidateViewModel;
            }
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
