using System.ComponentModel.DataAnnotations;

namespace WeightTracker.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime DateJoined { get; set; }
        public DayOfWeek WeekStart { get; set; }
        public Units PreferredUnits { get; set; }
        public DateTime DateModified { get; set; }
    }
}
