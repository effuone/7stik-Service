namespace Zhetistik.Core.Interfaces
{
    public interface ISchoolRepository : IAsyncRepository<School>
    {
        public Task<School> GetAsync(string name);
    }
}