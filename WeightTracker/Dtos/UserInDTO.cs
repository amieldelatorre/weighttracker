using WeightTracker.Models;

namespace WeightTracker.Dtos
{
    public class UserInDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DayOfWeek WeekStart { get; set; }
        public Units PreferredUnits { get; set; }
    }
}
