using Microsoft.EntityFrameworkCore;
using WeightTracker.Models.Weight;

namespace WeightTracker.Data
{
    public class PgWeightRepo : IWeightRepo
    {
        private readonly WeightTrackerDbContext _context;

        public PgWeightRepo(WeightTrackerDbContext context)
        {
            _context = context;
        }

        async public Task<bool> Add(Weight weight)
        {
            await _context.Weights.AddAsync(weight);
            int numberOfChangesSaved = _context.SaveChanges();
            return numberOfChangesSaved > 0;
        }

        async public Task<bool> WeightExistsForUserIdAndDate(int userId, DateOnly date)
        {
            Weight? weight = await _context.Weights.SingleOrDefaultAsync(weight => (weight.UserId == userId && weight.Date == date));
            return weight != null;
        }

        async public Task<Weight?> GetById(int userId, int weightId)
        {
            Weight? weight = await _context.Weights.SingleOrDefaultAsync(weight => weight.UserId == userId && weight.Id == weightId);
            return weight;
        }

        public IQueryable<Weight> GetAllByUserId(int userId)
        {
            IQueryable<Weight> weights = _context.Weights.Where(weight => weight.UserId == userId);
            return weights;
        }
    }
}
