using WeightTracker.Models;

namespace WeightTracker.Data
{
    public interface IUserRepo
    {
        bool Add(User user);
        bool EmailExists(string email);
    }
}
