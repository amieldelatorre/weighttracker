using System.ComponentModel.DataAnnotations;

namespace WeightTracker.Models
{
    public class WeightProgress
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public double Weight { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

    }
}
