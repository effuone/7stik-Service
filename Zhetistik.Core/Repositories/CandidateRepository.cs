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
                                @"select candidates.CandidateId, users.FirstName, users.LastName, candidates.Birthday, locations.LocationId, countries.CountryName, cities.CityName, candidates.GraduateYear
                    from Candidates as candidates, AspNetUsers as users, Schools as schools, Locations as locations, Countries as countries, Cities as cities
                    where candidates.ZhetistikUserId = users.Id
                    and locations.LocationId = candidates.LocationId
                    and locations.CountryId = countries.CountryId
                    and locations.CityId = cities.CityId
                    and candidates.CandidateId = @id";
                    var command = new SqlCommand(sql, (SqlConnection)connection);
                    command.Parameters.Add(new SqlParameter("@id", candidateId));
                    var reader = await command.ExecuteReaderAsync();
                    if(reader.HasRows is false)
                    {
                        return null;
                    }
                    await reader.ReadAsync();
                    candidateViewModel.CandidateId = candidateId;
                    candidateViewModel.FirstName = reader.GetString(1);
                    candidateViewModel.LastName = reader.GetString(2);
                    candidateViewModel.Birthday = Convert.IsDBNull(reader.GetDateTime(3)) is false ? reader.GetDateTime(3) : DateTime.MinValue;
                    candidateViewModel.Location = new LocationViewModel{ LocationId = reader.GetInt32(4), CountryName = reader.GetString(5), CityName = reader.GetString(6)};
                    candidateViewModel.GraduateYear = reader.GetDateTime(7);
                    connection.Close();
                }
                {
                    connection.Open();
                    string sql = 
                                        @"select schools.Schoolname, schools.FoundationYear, locations.LocationId, countries.CountryName, cities.CityName FROM
                    Schools as schools, Countries as countries, Cities as cities, Locations as locations, Candidates as candidates
                    where countries.CountryId = locations.CountryId and locations.CityId = locations.CityId 
                    and schools.LocationId = locations.LocationId
                    and candidates.SchoolId = schools.SchoolId
                    and candidates.CandidateId = @id";
                    var command = new SqlCommand(sql, (SqlConnection) connection);
                    command.Parameters.Add(new SqlParameter("@id", candidateId));
                    var reader = await command.ExecuteReaderAsync();
                    if(reader.HasRows is false)
                    {
                        candidateViewModel.School = null;
                    }

                    await reader.ReadAsync();
                    var schoolViewModel = new SchoolViewModel();
                    schoolViewModel.SchoolName =  reader.GetString(0);
                    schoolViewModel.FoundationYear =  reader.GetDateTime(1);
                    schoolViewModel.Location = new LocationViewModel{LocationId = reader.GetInt32(2), CountryName = reader.GetString(3), CityName = reader.GetString(4)};
                    candidateViewModel.School = schoolViewModel;
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
