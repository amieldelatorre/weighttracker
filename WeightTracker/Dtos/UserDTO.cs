using WeightTracker.Models;

namespace WeightTracker.Dtos
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime DateJoined { get; set; }
        public DayOfWeek WeekStart { get; set; }
        public Units PreferredUnits { get; set; }
        public DateTime DateModified { get; set; }
    }
}
