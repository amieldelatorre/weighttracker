using WeightTracker.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeightTracker.Models.User
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]

        public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public required string FirstName { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public required string LastName { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public required string Email { get; set; }
        [Required]
        [Column(TypeName = "varchar")]
        public required string Password { get; set; }
        [Required]
        [Column(TypeName = "date")]

        public required DateOnly DateOfBirth { get; set; }
        [Required]
        [Column(TypeName = "integer")]

        public required GenderEnum Gender { get; set; }
        [Required]
        [Column(TypeName = "double precision")]

        public required double Height { get; set; }
        [Required]
        [Column(TypeName = "timestamp with time zone")]
        public required DateTime DateCreated { get; set; }
        [Required]
        [Column(TypeName = "timestamp with time zone")]
        public required DateTime DateModified { get; set; }
        public List<WeightTracker.Models.Weight.Weight>? Weights { get; set; }
    }
}
