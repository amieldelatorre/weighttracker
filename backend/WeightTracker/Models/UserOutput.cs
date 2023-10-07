using System.Text.Json.Serialization;
using WeightTracker.Enums;

namespace WeightTracker.Models
{
    public class UserOutput
    {
        public int Id { get; set; }
        public  string FirstName { get; set; }
        public  string LastName { get; set; }
        public  string Email { get; set; }
        public  DateOnly DateOfBirth { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public  GenderEnum Gender { get; set; }
        public  double Height { get; set; }
        public  DateTime DateCreated { get; set; }
        public  DateTime DateModified { get; set; }

        public UserOutput(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            DateOfBirth = user.DateOfBirth;
            Gender = user.Gender;
            Height = user.Height;
            DateCreated = user.DateCreated;
            DateModified = user.DateModified;
        }

    }
}
