using WeightTracker.Data;

namespace WeightTracker.Models.Weight
{
    public class WeightCreate : ICreate
    {
        public required double UserWeight {  get; set; }
        public string? Description { get; set; }
        public required DateOnly Date {  get; set; }

        async public Task<bool> IsValid(IWeightRepo weightRepo, int userId)
        {
            await FindErrors(weightRepo, userId);
            return Errors.Count == 0;
        }

        async public Task FindErrors(IWeightRepo weightRepo, int userId)
        {
            // Validate user weight
            if (UserWeight <= 0)
                AddToErrors(nameof(UserWeight), "User weight cannot be less than or equal to 0");

            // Validate user and date combination is unique
            if (await weightRepo.WeightExistsForUserIdAndDate(userId, this.Date))
                AddToErrors(nameof(Date), "A weight already exists for this date");

            // Validate input date
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
            if (Date > dateToday)
                AddToErrors(nameof(Date), "Date cannot be greater than today");
        }

        public Weight CreateWeight(int userId)
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            Weight weight = new()
            {
                UserId = userId,
                UserWeight = UserWeight,
                Date = Date,
                Description = Description,
                DateCreated = now,
                DateModified = now,
            };
            return weight;
        }
    }
}
