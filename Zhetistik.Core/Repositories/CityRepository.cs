using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Zhetistik.Core.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly DapperContext _context;

        public CityRepository(DapperContext connection)
        {
            _context = connection;
        }

        public async Task<int> CreateAsync(City model)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();

                string sql = @"INSERT INTO Cities (CityName) VALUES (@CityName) SET @CityId = SCOPE_IDENTITY();";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@CityName", model.CityName));

                var outputParam = new SqlParameter
                {
                    ParameterName = "@CityId",
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
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var result = await connection.DeleteAsync<City>(new City { CityId = id });
                return result;
            }
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var models = (await connection.GetAllAsync<City>()).ToList();
                return models;
            }
        }

        public async Task<City> GetAsync(string cityName)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var model = (await connection.QueryAsync<City>($"select* from Cities where CityName = '{cityName}'")).FirstOrDefault();
                return model;
            }
        }

        public async Task<City> GetAsync(int id)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var model = await connection.GetAsync<City>(id);
                return model;
            }
        }

        public async Task<IEnumerable<City>> GetAllAsync(string countryName)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var city = (await connection.QueryAsync<City>("SELECT ct.CityId, ct.CityName FROM Locations as loc, Cities as ct, Countries as coun WHERE loc.CityID = ct.CityId and loc.CountryID = coun.CountryId and coun.CountryName = 'countryName'"));
                return city;
            }
        }

        public async Task<bool> UpdateAsync(int id, City model)
        {
            using(var connection = _context.CreateConnection())
            {
                string updateQuery = @"UPDATE [dbo].[Cities] SET CityName = @cityName WHERE CityId = @id";
                var result = await connection.ExecuteAsync(updateQuery, new
                {
                    model.CityName,
                    id
                });
                if(result>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
             
        }
    }
}