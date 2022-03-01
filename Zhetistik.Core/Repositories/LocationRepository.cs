using Zhetistik.Core.Interfaces;
using Zhetistik.Data.ViewModels;

namespace Zhetistik.Core.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DapperContext _context;

        public LocationRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> CreateAsync(Location model)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();

                string sql = @"INSERT INTO Locations (CountryId, CityId) VALUES (@CountryId, @CityId) SET @LocationId = SCOPE_IDENTITY();";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@CountryId", model.CountryId));
                command.Parameters.Add(new SqlParameter("@CityId", model.CityId));

                var outputParam = new SqlParameter
                {
                    ParameterName = "@LocationId",
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
                var result = await connection.DeleteAsync<Location>(new Location { LocationId = id });
                return result;
            }
        }

        public async Task<IEnumerable<Location>> GetAllAsync(string countryName)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var models = (await connection.GetAllAsync<Location>()).ToList();
                return models;
            }
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var models = (await connection.GetAllAsync<Location>()).ToList();
                return models;
            }
        }

        public async Task<IEnumerable<LocationViewModel>> GetAllViewModelsAsync()
        {
            string sql = "select loc.LocationId, co.CountryName, ct.CityName from Locations as loc, Cities as ct, Countries as co where loc.CityId = ct.CityId and co.CountryId = loc.CountryId";
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new SqlCommand(sql, (SqlConnection) connection);
                var reader = await command.ExecuteReaderAsync();
                List<LocationViewModel> locationViewModels = new List<LocationViewModel>();
                while(await reader.ReadAsync())
                {
                    locationViewModels.Add(new LocationViewModel{
                        LocationId = reader.GetInt32(0),
                        CountryName = reader.GetString(1),
                        CityName = reader.GetString(2)
                    });
                }
                return locationViewModels;
            }
        }

        public async Task<Location> GetAsync(string countryName, string cityName)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var model = (await connection.QueryAsync<Location>($"")).FirstOrDefault();
                return model;
            }
        }

        public async Task<Location> GetAsync(int id)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var model = await connection.GetAsync<Location>(id);
                return model;
            }
        }

        public async Task<LocationViewModel> GetViewModelAsync(string countryName, string cityName)
        {
            using(var connection = _context.CreateConnection())
            {
                string sql = $"select loc.LocationId, co.CountryName, ct.CityName from Locations as loc, Cities as ct, Countries as co where loc.CityId = ct.CityId and co.CountryId = loc.CountryId and ct.CityName=@cityName and co.CountryName=@countryName";
                var result = await connection.QueryFirstAsync<LocationViewModel>(sql, new
                {
                    cityName,
                    countryName
                });
                return result;
            }
        }

        public async Task<LocationViewModel> GetViewModelAsync(int id)
        {
            string sql = @"select loc.LocationId, co.CountryName, ct.CityName
                        from Locations as loc, Cities as ct, Countries as co
                        where loc.CityId = ct.CityId and co.CountryId = loc.CountryId and loc.LocationId = @id";
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var result = await connection.QueryFirstAsync<LocationViewModel>(sql, new
                {
                    id
                });
                return result;
            }

        }

        public async Task<Location> GetAsync(int countryId, int cityId)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var model = (await connection.QueryAsync<Location>($"select* from Locations where CountryId = {countryId} and CityId = {cityId}")).FirstOrDefault();
                return model;
            }
        }

        public async Task<bool> UpdateAsync(int id, Location model)
        {
            using(var connection = _context.CreateConnection())
            {
                string updateQuery = @"UPDATE [dbo].[Locations] SET CountryId = @countryId, CityId = @cityId WHERE LocationId = @id";
                var result = await connection.ExecuteAsync(updateQuery, new
                {
                    model.CountryId,
                    model.CityId,
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