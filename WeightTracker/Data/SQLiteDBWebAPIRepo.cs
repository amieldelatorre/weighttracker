using WeightTracker.Models;
using WeightTracker.Exceptions;

namespace WeightTracker.Data
{
    public class SQLiteDBWebAPIRepo : IWeightTrackerAPIRepo
    {
        public readonly WeightTrackerDBContext _dbContext;
        public SQLiteDBWebAPIRepo(WeightTrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User AddUser(User user)
        {
            User? existingUser = GetUserByUsername(user.UserName);
            if (existingUser != null) throw new UsernameAlreadyExistsException("Username already exists.", user.UserName);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return user;
        }

        public WeightProgress AddWeightProgress(WeightProgress weightProgress)
        {
            _dbContext.WeightProgresses.Add(weightProgress);
            _dbContext.SaveChanges();
            return weightProgress;
        }

        public Boolean CheckUserPassword(String username, string password)
        {
            User? user = GetUserByUsername(username);
            if (user == null) throw new UsernameNotFoundException("Username and password do not match.");

            if (user.Password != password) return false;
            return true;
        }

        public Boolean CheckUserPassword(User user, String password)
        {
            if (user.Password != password) return false;
            return true;
        }

        public void DeleteUser(User user)
        {
            _dbContext.Remove(user);
            _dbContext.SaveChanges();
        }

        public void DeleteWeightProgress(WeightProgress weightProgress)
        {
            _dbContext.Remove(weightProgress);
            _dbContext.SaveChanges();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _dbContext.Users.ToList<User>();
        }

        public User? GetUserByUsername(String username)
        {
            return _dbContext.Users.SingleOrDefault(e => e.UserName == username);
        }

        public IEnumerable<WeightProgress> GetAllWeights()
        {
            return _dbContext.WeightProgresses.ToList<WeightProgress>();
        }

        public User? GetUser(int userId)
        {
            return _dbContext.Users.FirstOrDefault(e => e.Id == userId);
        }

        public WeightProgress? GetWeight(int weightId)
        {
            return _dbContext.WeightProgresses.FirstOrDefault(e => e.Id == weightId);
        }

        public IEnumerable<WeightProgress> GetWeightProgressByUserId(int userId)
        {
            return _dbContext.WeightProgresses.Where(e => e.UserId == userId);
        }

        public User UpdateUser(User user)
        {
            _dbContext.Update(user);
            _dbContext.SaveChanges();
            return user;
        }

        public WeightProgress UpdateWeightProgress(WeightProgress weightProgress)
        {
            _dbContext.Update(weightProgress);
            _dbContext.SaveChanges();
            return weightProgress;
        }
    }
}
