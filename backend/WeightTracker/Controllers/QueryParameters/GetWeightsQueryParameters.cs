namespace WeightTracker.Controllers.QueryParameters
{
    public class GetWeightsQueryParameters
    {
        public int Limit { get; set; } = 100;
        public int Offset { get; set; } = 0;
        public DateOnly? DateFrom { get; set; } = null;
        public DateOnly? DateTo { get; set;} = null;
    }
}
