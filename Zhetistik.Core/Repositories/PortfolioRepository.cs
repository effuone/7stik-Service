global using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Zhetistik.Core.DataAccess;
using Zhetistik.Core.Interfaces;
using Zhetistik.Data.Context;

namespace Zhetistik.Data.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ZhetistikAppContext _context;
        private readonly DapperContext _dapper;

        public PortfolioRepository(ZhetistikAppContext context, DapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task<int> CreateAsync(Portfolio model)
        {
            await _context.Portfolios.AddAsync(model);
            await _context.SaveChangesAsync();
            return model.PortfolioId;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var model = await _context.Candidates.FindAsync(id);
            var result = _context.Candidates.Remove(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Portfolio>> GetAllAsync()
        {
            return await _context.Portfolios.ToListAsync();
        }

        public async Task<Portfolio> GetAsync(int id)
        {
            return await _context.Portfolios.FindAsync(id);
        }

        public async Task<Portfolio> GetPortfolioByCandidateAsync(int candidateId)
        {
            using(var connection = _dapper.CreateConnection())
            {
                connection.Open();
                var portfolio = (await connection.QueryAsync<Portfolio>($"SELECT* from Portfolios WHERE CandidateId = {candidateId}")).First();
                return portfolio;
            }
        }

        public async Task<bool> UpdateAsync(int id, Portfolio model)
        {
            var existingModel = await _context.Portfolios.FindAsync(id);
            model.PortfolioId = id;
            _context.Portfolios.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}