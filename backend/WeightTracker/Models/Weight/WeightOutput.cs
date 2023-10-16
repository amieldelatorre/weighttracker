namespace WeightTracker.Models.Weight
{
    public class WeightOutput
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double UserWeight { get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public WeightOutput(Weight weight)
        {
            Id = weight.Id;
            UserId = weight.UserId;
            UserWeight = weight.UserWeight;
            Description = weight.Description;
            Date = weight.Date;
            DateCreated = weight.DateCreated;
            DateModified = weight.DateModified;
        }
    }
}
