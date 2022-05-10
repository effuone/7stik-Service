using System.Data;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;
using Zhetistik.Core.DataAccess;
using Zhetistik.Core.Interfaces;

namespace Zhetistik.Core.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly DapperContext _dapper;

        public PortfolioRepository(DapperContext dapper)
        {
            _dapper = dapper;
        }

        public async Task<int> CreateAsync(Portfolio model)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                string sql = @"INSERT INTO Portfolios (CandidateId, IsPublished) VALUES (@candidateId, 0) SET @PortfolioId = SCOPE_IDENTITY();";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@candidateId", model.CandidateId));
                var outputParam = new SqlParameter
                {
                    ParameterName = "@PortfolioId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                return (int)outputParam.Value;
            }
        }
        public async Task<int> CreatePortfolioWithCandidateAsync(int candidateId)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                string sql = @"INSERT INTO Portfolios (CandidateId, IsPublished) VALUES (@candidateId, 0) SET @PortfolioId = SCOPE_IDENTITY();";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@candidateId", candidateId));
                var outputParam = new SqlParameter
                {
                    ParameterName = "@PortfolioId",
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
                var result = await connection.DeleteAsync<Portfolio>(new Portfolio { PortfolioId = id });
                return result;
            }
        }

        public async Task<IEnumerable<Portfolio>> GetAllAsync()
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var models = (await connection.GetAllAsync<Portfolio>()).ToList();
                return models;
            }
        }

        public async Task<Portfolio> GetAsync(int id)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var model = await connection.GetAsync<Portfolio>(id);
                return model;
            }
        }

        public async Task<Portfolio> GetPortfolioByCandidateAsync(int candidateId)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var model = (await connection.QueryAsync<Portfolio>($"select* from Portfolios where CandidateId = {candidateId}")).FirstOrDefault();
                return model;
            }
        }

        public async Task<bool> UpdateAsync(int id, Portfolio model)
        {
            using(var connection = _dapper.CreateConnection())
            {
                string updateQuery = @"UPDATE [dbo].[Portfolios] SET CandidateId = @candidateId, IsPublished = @isPublished WHERE PortfolioId = @id";
                var result = await connection.ExecuteAsync(updateQuery, new
                {
                    model.CandidateId,
                    model.IsPublished,
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