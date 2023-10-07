using WeightTracker.Models;

namespace WeightTracker.Data
{
    public class PgUserRepo: IUserRepo
    {
        private readonly WeightTrackerDbContext _context;

        public PgUserRepo( WeightTrackerDbContext context)
        {
            _context = context;
        }

        public bool Add(User user)
        {
            _context.Users.Add(user);
            int numberOfChangesSaved = _context.SaveChanges();
            return numberOfChangesSaved > 0;
        }

        public bool EmailExists(string email)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            return user != null;
        }
    }
}
