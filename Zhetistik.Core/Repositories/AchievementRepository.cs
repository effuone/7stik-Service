global using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;
using Zhetistik.Core.DataAccess;
using Zhetistik.Core.Interfaces;
using Zhetistik.Data.Context;
using Zhetistik.Data.ViewModels;

namespace Zhetistik.Data.Repositories
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly ZhetistikAppContext _context;
        private readonly DapperContext _dapper;

        public AchievementRepository(ZhetistikAppContext context, DapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task<int> CreateAsync(Achievement model)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();

                string sql = @"INSERT INTO Achievements 
                (PortfolioId, AchievementName, Description, FileModelId, AchievementTypeId) 
                VALUES (@portfolioId, @AchievementName, @Description, @fileId, @typeId) 
                SET @AchievementId = SCOPE_IDENTITY();";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@portfolioId", model.PortfolioId));
                command.Parameters.Add(new SqlParameter("@AchievementName", model.AchievementName));
                command.Parameters.Add(new SqlParameter("@Description", model.Description));
                command.Parameters.Add(new SqlParameter("@fileId", model.FileModel.Id));
                command.Parameters.Add(new SqlParameter("@typeId", model.AchievementTypeId));

                var outputParam = new SqlParameter
                {
                    ParameterName = "@AchievementId",
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
                var result = await connection.DeleteAsync<Achievement>(new Achievement { AchievementId = id });
                return result;
            }
        }

        public Task<IEnumerable<AchievementViewModel>> GetAchievementsByCandidateAsync(int candidateId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Achievement>> GetAchievementsByPortfolioAsync(int portfolioId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AchievementViewModel>> GetAllAchievementViewModelsAsync()
        {
                        string sql = @"select a.AchievementId, aty.AchievementTypeName, a.AchievementName, a.Description, fl.Path
            from Achievements as a, AchievementTypes as aty, FileModels as fl
            where a.AchievementTypeId = aty.AchievementTypeId and a.FileModelId = fl.Id";
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var achievements = await connection.QueryAsync<AchievementViewModel>(sql);
                return achievements;
            }
        }

        public Task<IEnumerable<Achievement>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Achievement> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, Achievement model)
        {
            throw new NotImplementedException();
        }
    }
}