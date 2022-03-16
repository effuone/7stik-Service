using Zhetistik.Core.Interfaces;
using Zhetistik.Data.Context;

namespace Zhetistik.Core.Repositories
{
    public class AchievementTypeRepository : IAchievementTypeRepository
    {
        private readonly ZhetistikAppContext _context;

        public AchievementTypeRepository(ZhetistikAppContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(AchievementType model)
        {
            await _context.AchievementTypes.AddAsync(model);
            await _context.SaveChangesAsync();
            return model.AchievementTypeId;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var model = await _context.AchievementTypes.FindAsync(id);
            _context.AchievementTypes.Remove(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AchievementType>> GetAllAsync()
        {
            return await _context.AchievementTypes.ToListAsync();
        }

        public async Task<AchievementType> GetAsync(int id)
        {
            return await _context.AchievementTypes.FindAsync(id);
        }

        public async Task<AchievementType> GetAsync(string name)
        {
            return await _context.AchievementTypes.Where(x=>x.AchievementTypeName == name).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(int id, AchievementType model)
        {
            var existingAchievement = await _context.AchievementTypes.FindAsync(id);
            existingAchievement.AchievementTypeName = model.AchievementTypeName;
            _context.AchievementTypes.Update(existingAchievement);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}