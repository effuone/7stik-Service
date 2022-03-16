namespace Zhetistik.Core.Interfaces
{
    public interface IAchievementTypeRepository : IAsyncRepository<AchievementType>
    {
         public Task<AchievementType> GetAsync(string name);
         
    }
}