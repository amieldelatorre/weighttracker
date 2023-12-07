using System.Numerics;

namespace WeightTracker.Models
{
    public class PaginatedResult<T>
    {
        public List<T> Results { get; set; } = [];
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string? Next { get; set; } = null;
    }
}
