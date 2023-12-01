using Microsoft.EntityFrameworkCore;
using WeightTracker.Models.User;

namespace WeightTracker.Data
{
    public class PgUserRepo: IUserRepo
    {
        private readonly WeightTrackerDbContext _context;

        public PgUserRepo( WeightTrackerDbContext context)
        {
            _context = context;
        }

        async public Task<bool> Add(User user)
        {
            await _context.Users.AddAsync(user);
            int numberOfChangesSaved = _context.SaveChanges();
            return numberOfChangesSaved > 0;
        }

        async public Task<bool> EmailExists(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null;
        }

        async public Task<User?> GetByEmail(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<bool> IsValidLogin(string email, string password)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u =>u.Email == email);
            return (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password));
        }
    }
}
