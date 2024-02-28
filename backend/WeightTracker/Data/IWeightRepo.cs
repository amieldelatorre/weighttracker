using WeightTracker.Models.Weight;

namespace WeightTracker.Data
{
    public interface IWeightRepo
    {
        Task<bool> Add(Weight weight);
        Task<bool> WeightExistsForUserIdAndDate(int userId, DateOnly date);
        Task<Weight?> GetWeightByUserIdAndDate(int userId, DateOnly date);
        Task<Weight?> GetById(int userId, int weightId);
        IQueryable<Weight> GetAllByUserId(int userId);
        Task<bool> Update(Weight weight);
    }
}
