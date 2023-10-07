using Microsoft.EntityFrameworkCore;
using WeightTracker.Models;

namespace WeightTracker.Data
{
    public class WeightTrackerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public WeightTrackerDbContext(DbContextOptions<WeightTrackerDbContext> options) : base(options) { }
    }
}
