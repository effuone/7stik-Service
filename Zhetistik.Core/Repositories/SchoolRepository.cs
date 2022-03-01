namespace Zhetistik.Core.Repositories
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly DapperContext _context;

        public SchoolRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(School model)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();

                string sql = @"INSERT INTO Schools (LocationId, SchoolName, Image, FoundationYear) VALUES (@locationId, @schoolName, @image, @year) SET @SchoolId = SCOPE_IDENTITY();";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@locationId", model.LocationId));
                command.Parameters.Add(new SqlParameter("@schoolName", model.SchoolName));
                command.Parameters.Add(new SqlParameter("@image", model.Image));
                command.Parameters.Add(new SqlParameter("@year", model.FoundationYear));

                var outputParam = new SqlParameter
                {
                    ParameterName = "@SchoolId",
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
                var result = await connection.DeleteAsync<School>(new School { SchoolId = id });
                return result;
            }
        }

        public async Task<IEnumerable<School>> GetAllAsync()
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var models = (await connection.GetAllAsync<School>()).ToList();
                return models;
            }
        }

        public async Task<School> GetAsync(string name)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var model = (await connection.QueryAsync<School>($"select* from Schools where SchoolName = '{name}'")).FirstOrDefault();
                return model;
            }
        }

        public async Task<School> GetAsync(int id)
        {
            using(var connection = _context.CreateConnection())
            {
                connection.Open();
                var model = (await connection.QueryAsync<School>($"select* from Schools where SchoolId = {id}")).FirstOrDefault();
                return model;
            }
        }

        public async Task<bool> UpdateAsync(int id, School model)
        {
            using(var connection = _context.CreateConnection())
            {
                string updateQuery = @"UPDATE [dbo].[Schools] SET LocationId = @locationId, SchoolName = @schoolName, Image = @image, FoundationYear = @year WHERE SchoolId = @id";
                var result = await connection.ExecuteAsync(updateQuery, new
                {
                    model.LocationId,
                    model.SchoolName,
                    model.Image,
                    model.FoundationYear,
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