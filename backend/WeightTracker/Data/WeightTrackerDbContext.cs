using Microsoft.EntityFrameworkCore;
using WeightTracker.Models.User;
using WeightTracker.Models.Weight;

namespace WeightTracker.Data
{
    public class WeightTrackerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Weight> Weights { get; set; }
        public WeightTrackerDbContext(DbContextOptions<WeightTrackerDbContext> options) : base(options) { }
    }
}
