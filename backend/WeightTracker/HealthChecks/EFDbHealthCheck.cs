using Microsoft.Extensions.Diagnostics.HealthChecks;
using WeightTracker.Controllers;
using WeightTracker.Data;

namespace WeightTracker.HealthChecks
{
    public class EFDbHealthCheck : IHealthCheck
    {
        private readonly WeightTrackerDbContext _context;
        private readonly ILogger<EFDbHealthCheck> _logger;
        public EFDbHealthCheck(ILogger<EFDbHealthCheck> logger, WeightTrackerDbContext context) 
        {
            _logger = logger;
            _context = context;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken) 
        {
            _logger.LogDebug("Health check on database connection.");
            if (_context.Database.CanConnect())
                return Task.FromResult(HealthCheckResult.Healthy("Able to connect to database."));

            return Task.FromResult(HealthCheckResult.Unhealthy("Unable to connect to database"));
        }
    }
}
