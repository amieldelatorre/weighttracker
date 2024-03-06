using WeightTracker.Data;

namespace WeightTracker.Models.Weight
{
    public class WeightUpdate : ICreate
    {
        public required double UserWeight { get; set; }
        public string? Description { get; set; }
        public required DateOnly Date { get; set; }

        async public Task<bool> IsValid(IWeightRepo weightRepo, int userId)
        {
            FindErrors(weightRepo, userId);
            return Errors.Count == 0;
        }

        public void FindErrors(IWeightRepo weightRepo, int userId)
        {
            // Validate user weight
            if (UserWeight <= 0)
                AddToErrors(nameof(UserWeight), "User weight cannot be less than or equal to 0");

            // Validate input date
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
            if (Date > dateToday)
                AddToErrors(nameof(Date), "Date cannot be greater than today");
        }
    }
}
