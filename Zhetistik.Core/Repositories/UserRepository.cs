// using Zhetistik.Data.Context;

// namespace Zhetistik.Core.Repositories
// {
//     public class UserRepository: IUserRepository
//     {
//         private readonly ZhetistikAppContext _context;

//         public UserRepository(ZhetistikAppContext context)
//         {
//             _context = context;
//         }

//         public async Task<int> CreateAsync(User model)
//         {
//             var result = await _context.Set<User>().AddAsync(model);
//             await _context.SaveChangesAsync();
//             return result.Entity.UserId;
//         }

//         public async Task<bool> DeleteAsync(long id)
//         {
//             var model = await _context.Set<User>().FirstOrDefaultAsync(x=>x.UserId == id);
//             if(model is null)
//             {
//                 return false;
//             }
//             _context.Users.Remove(model);
//             await _context.SaveChangesAsync();
//             return true;
//         }

//         public async Task<IEnumerable<User>> GetAllAsync()
//         {
//             return await _context.Set<User>().ToListAsync();
//         }

//         public async Task<User> GetAsync(int id)
//         {
//            var result = await _context.Set<User>().FirstOrDefaultAsync(x=>x.UserId == id);
//            if(result is not null)
//            {
//                return result;
//            }
//            return null;
//         }

//         public async Task<bool> UpdateAsync(long id, User model)
//         {
//             var existingModel = await _context.Set<User>().FirstOrDefaultAsync(x=>x.UserId == id);
//             if(existingModel is null)
//             {
//                 return false;
//             }
//             existingModel = model;
//             _context.Users.Update(existingModel);
//             return true;
//         }
//     }
// }