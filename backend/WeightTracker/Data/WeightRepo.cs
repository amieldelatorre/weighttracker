using Microsoft.EntityFrameworkCore;
using WeightTracker.Models.Weight;

namespace WeightTracker.Data
{
    public class WeightRepo : IWeightRepo
    {
        private readonly WeightTrackerDbContext _context;

        public WeightRepo(WeightTrackerDbContext context)
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

        async public Task<Weight?> GetWeightByUserIdAndDate(int userId, DateOnly date)
        {
            Weight? weight = await _context.Weights.SingleOrDefaultAsync(weight => (weight.UserId == userId && weight.Date == date));
            return weight;
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

        public async Task<bool> Update(Weight weight)
        {
            _context.Update(weight);
            int saveResults = await _context.SaveChangesAsync();
            return saveResults > 0;
        }

        public async Task<bool> Delete(Weight weight)
        {
            _context.Remove(weight);
            int saveResults = await _context.SaveChangesAsync();
            return saveResults > 0;
        }
    }
}
