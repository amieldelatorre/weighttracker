using Microsoft.EntityFrameworkCore;
using WeightTracker.Models;

namespace WeightTracker.Data
{
    public class WeightTrackerDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=WeightTrackerDB.sqlite");
        }
        public WeightTrackerDBContext(DbContextOptions<WeightTrackerDBContext> options): base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<WeightProgress> WeightProgresses { get; set; }
    }
}
