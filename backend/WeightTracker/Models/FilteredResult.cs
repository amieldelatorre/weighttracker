namespace WeightTracker.Models
{
    public class FilteredResult<T>
    {
        public int Total { get; set; }
        public List<T> Results { get; set; }= [];
    }
}
