global using Dapper;
using System.Data;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;
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
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();

                string sql = @"INSERT INTO Candidates (ZhetistikUserId) VALUES (@userId) SET @candidateId = SCOPE_IDENTITY();";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@userId", model.ZhetistikUserId));

                var outputParam = new SqlParameter
                {
                    ParameterName = "@candidateId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                return (int)outputParam.Value;
            }
        }

        public async Task<int> CreateAsync(string zhetistikUserId)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                string sql = @"INSERT INTO Candidates (ZhetistikUserId) VALUES (@userId) SET @CandidateId = SCOPE_IDENTITY();";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@userId", zhetistikUserId));
                var outputParam = new SqlParameter
                {
                    ParameterName = "@CandidateId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                return (int)outputParam.Value;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var result = await connection.DeleteAsync<Candidate>(new Candidate { CandidateId = id });
                return result;
            }
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
            string procedure = $"EXEC GetCandidates @Id = {candidateId}";
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var command = new SqlCommand(procedure, (SqlConnection)connection);
                var reader = await command.ExecuteReaderAsync();
                var candidates = new List<CandidateViewModel>();
                if(reader.HasRows)
                {
                    while(await reader.ReadAsync())
                    {
                        var cwm = new CandidateViewModel();
                        cwm.CandidateId = candidateId;
                        cwm.Username= reader.GetString(0);
                        cwm.FirstName= reader.GetString(1);
                        cwm.LastName= reader.GetString(2);
                        cwm.Email= reader.GetString(3);
                        cwm.PhoneNumber= reader.GetString(4);   
                        candidates.Add(cwm);   
                    }
                }
                connection.Close();
                return candidates.First();
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