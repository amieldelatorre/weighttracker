using WeightTracker.Models;

namespace WeightTracker.Data
{
    public interface IWeightTrackerAPIRepo
    {
        IEnumerable<User> GetAllUsers();
        User? GetUserByUsername(String username);
        User? GetUser(int userId);
        User AddUser(User user);   
        User UpdateUser(User user);
        void DeleteUser(User user);
        IEnumerable<WeightProgress> GetAllWeights();
        WeightProgress? GetWeight(int weightId);
        IEnumerable<WeightProgress> GetWeightProgressByUserId(int userId);
        WeightProgress AddWeightProgress(WeightProgress weightProgress);
        WeightProgress UpdateWeightProgress(WeightProgress weightProgress);
        void DeleteWeightProgress(WeightProgress weightProgress);
    }
}
