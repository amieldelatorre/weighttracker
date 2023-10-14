using WeightTracker.Models.User;

namespace WeightTracker.Data
{
    public interface IUserRepo
    {
        Task<bool> Add(User user);
        Task<bool> EmailExists(string email);
        Task<User?> GetByEmail(string email);
        Task<bool> IsValidLogin(string email, string password);
    }
}
