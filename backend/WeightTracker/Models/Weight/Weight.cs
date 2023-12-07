using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeightTracker.Models.Weight
{
    [Index(nameof(UserId), nameof(Date), IsUnique = true)]
    public class Weight
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required int UserId { get; set; }
        [Required]
        [Column(TypeName = "double precision")]
        public required double UserWeight { get; set; }
        [Column(TypeName = "text")]
        public string? Description {  get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateOnly Date {  get; set; }
        [Required]
        [Column(TypeName = "timestamp with time zone")]
        public required DateTime DateCreated { get; set; }
        [Required]
        [Column(TypeName = "timestamp with time zone")]
        public required DateTime DateModified { get; set; }
    }
}
