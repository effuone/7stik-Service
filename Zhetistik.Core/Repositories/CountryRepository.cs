using System.Data;
using System.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;
using Zhetistik.Core.DataAccess;
using Zhetistik.Core.Interfaces;

namespace Zhetistik.Core.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DapperContext _context;

        public CountryRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> CreateAsync(Country model)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();

                string sql = @"INSERT INTO Countries (CountryName) VALUES (@CountryName) SET @CountryId = SCOPE_IDENTITY();";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@CountryName", model.CountryName));

                var outputParam = new SqlParameter
                {
                    ParameterName = "@CountryId",
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
                var result = await connection.DeleteAsync<Country>(new Country { CountryId = id });
                return result;
            }
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var models = (await connection.GetAllAsync<Country>()).ToList();
                return models;
            }
        }

        public async Task<Country> GetAsync(string countryName)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var model = (await connection.QueryAsync<Country>($"select* from Countries where CountryName = {countryName}")).FirstOrDefault();
                return model;
            }
        }

        public async Task<Country> GetAsync(int id)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var model = await connection.GetAsync<Country>(id);
                return model;
            }
        }

        public async Task<bool> UpdateAsync(int id, Country model)
        {
            using(var connection = _context.CreateConnection())
            {
                string updateQuery = @"UPDATE [dbo].[Countries] SET CountryName = @countryName WHERE CountryId = @id";
                var result = await connection.ExecuteAsync(updateQuery, new
                {
                    model.CountryName,
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