global using Microsoft.EntityFrameworkCore;
using Zhetistik.Core.Interfaces;
using Zhetistik.Data.Context;

namespace Zhetistik.Data.Repositories
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly ZhetistikAppContext _context;

        public AchievementRepository(ZhetistikAppContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Achievement model)
        {
            await _context.Achievements.AddAsync(model);
            await _context.SaveChangesAsync();
            return model.AchievementId;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var model = await _context.Achievements.FindAsync(id);
            var result = _context.Achievements.Remove(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Achievement>> GetAllAsync()
        {
            return await _context.Achievements.ToListAsync();
        }

        public async Task<Achievement> GetAsync(int id)
        {
            return await _context.Achievements.FindAsync(id);
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