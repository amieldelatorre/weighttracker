using WeightTracker.Models;

namespace WeightTracker.Data
{
    public interface IUserRepo
    {
        Task<bool> Add(User user);
        bool EmailExists(string email);
    }
}
