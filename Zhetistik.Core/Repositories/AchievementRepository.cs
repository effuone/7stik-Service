global using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
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
            await _context.Achievements.AddAsync(model);
            await _context.SaveChangesAsync();
            return model.AchievementId;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                string sql = @$"delete from Achievements
                where AchievementId = {id}";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                var result = await command.ExecuteNonQueryAsync();
                connection.Close();
            }
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                string sql = @$"delete from FileModels
                where Id = {id}";
                var command = new SqlCommand(sql, (SqlConnection)connection);
                connection.Close();
            }
            return true;
        }

        public Task<IEnumerable<AchievementViewModel>> GetAchievementsByCandidateAsync(int candidateId)
        {
            throw new NotImplementedException();
        }

        // public async Task<IEnumerable<AchievementViewModel>> GetAchievementsByCandidateAsync(int candidateId)
        // {
        //     using(var connection = _dapper.CreateConnection())
        //     {
        //         connection.Open();
        //          string sql = 
        //                     @"select achievements.AchievementId, achievements.AchievementName, achievementTypes.[AchievementTypeName], achievements.[Description], achievements.FileModelId
        //         from Achievements as achievements, AchievementTypes as achievementTypes, Candidates as candidates, Portfolios as portfolios
        //         where achievements.PortfolioId = portfolios.PortfolioId 
        //         and achievementTypes.AchievementTypeId = achievements.AchievementTypeId
        //         and portfolios.CandidateId = candidates.CandidateId
        //         and candidates.CandidateId = @id;";

        //         var command = new SqlCommand(sql, (SqlConnection)connection);
        //         command.Parameters.Add(new SqlParameter("@id", candidateId));
        //         var reader = await command.ExecuteReaderAsync();
        //         var list = new List<AchievementViewModel>();
        //         if(reader.HasRows)
        //         {
        //             while(await reader.ReadAsync())
        //             {
        //                 var achievement = new Zhetistik.Data.ViewModels.AchievementViewModel();
        //                 achievement.AchievementId = reader.GetInt32(0);
        //                 achievement.AchievementName = reader.GetString(1);
        //                 achievement.AchievementTypeName = reader.GetString(2);
        //                 achievement.Description = reader.GetString(3);
        //                 if(await reader.IsDBNullAsync(4) is false)
        //                 {
        //                     achievement.FilePath = (await _fileRepository.GetFileAsync(reader.GetInt32(4))).Path;
        //                 }
        //                 list.Add(achievement);
        //             } 
        //         }                    
        //         connection.Close();
        //         return list;
        //     }  
        // }

        public async Task<IEnumerable<Achievement>> GetAchievementsByPortfolioAsync(int portfolioId)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var achievements = (await connection.QueryAsync<Achievement>($"select* from Achievements where PortfolioId = {portfolioId}"));
                return achievements;
            }
        }

        public async Task<IEnumerable<AchievementViewModel>> GetAllAchievementViewModelsAsync()
        {
            var list = new List<AchievementViewModel>();
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                string sql = @"SELECT a.AchievementId, at.AchievementTypeName, a.AchievementName, a.Description, fm.Path 
                FROM Achievements as a, FileModels as fm, AchievementTypes as at 
                WHERE a.AchievementTypeId = at.AchievementTypeId and a.FileModelId = fm.Id";
                 var command = new SqlCommand(sql, (SqlConnection)connection);
                var reader = await command.ExecuteReaderAsync();
                if(reader.HasRows)
                {
                    while(await reader.ReadAsync())
                    {
                        var achievement = new Zhetistik.Data.ViewModels.AchievementViewModel();
                        achievement.AchievementId = reader.GetInt32(0);
                        achievement.AchievementName = reader.GetString(1);
                        achievement.AchievementTypeName = reader.GetString(2);
                        achievement.Description = reader.GetString(3);
                        if(await reader.IsDBNullAsync(4) is false)
                        {
                            achievement.FilePath = reader.GetString(4);
                        }
                        list.Add(achievement);
                    } 
                }                    
                connection.Close();
                return list;
            }
            
        }

        public async Task<IEnumerable<Achievement>> GetAllAsync()
        {
            return await _context.Achievements.ToListAsync();
        }

        public async Task<Achievement> GetAsync(int id)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var model = (await connection.QueryAsync<Achievement>($"select* from Achievements where AchievementId = {id}")).FirstOrDefault();
                return model;
            }
        }

        public async Task<bool> UpdateAsync(int id, Achievement model)
        {
            var existingModel = await _context.Achievements.FindAsync(id);
            model.AchievementId = id;
            _context.Achievements.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}