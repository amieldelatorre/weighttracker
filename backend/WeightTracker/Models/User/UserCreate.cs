using System.Text.Json.Serialization;
using WeightTracker.Data;
using WeightTracker.Enums;

namespace WeightTracker.Models.User
{
    public class UserCreate
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required DateOnly DateOfBirth { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required GenderEnum Gender { get; set; }
        public required double Height { get; set; }
        private Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();

        async public Task<bool> IsValid(IUserRepo userRepo)
        {
            await FindErrors(userRepo);
            return Errors.Count == 0;
        }

        async public Task FindErrors(IUserRepo userRepo)
        {
            string nullOrEmptyMessage = "Cannot be null or empty";
            string emailExistsMessage = "Email already exists";

            // Validate names
            if (string.IsNullOrEmpty(FirstName.Trim()))
                AddToErrors(nameof(FirstName), nullOrEmptyMessage);
            if (string.IsNullOrEmpty(LastName.Trim()))
                AddToErrors(nameof(LastName), nullOrEmptyMessage);

            // Validate email
            if (string.IsNullOrEmpty(Email.Trim()))
                AddToErrors(nameof(Email), nullOrEmptyMessage);
            if (await userRepo.EmailExists(Email.Trim()))
                AddToErrors(nameof(Email), emailExistsMessage);

            // Validate password
            if (string.IsNullOrEmpty(Password.Trim()) || Password.Trim().Length < 8)
                AddToErrors(nameof(Password), "Must be a string with a minimum length of 8");

            // Validate date of birth
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
            if (DateOfBirth > dateToday)
                AddToErrors(nameof(DateOfBirth), "Date of birth cannot be greater than today");

            // Validate gender
            if (!Enum.IsDefined(typeof(GenderEnum), Gender))
                AddToErrors(nameof(Gender), "Gender values are MALE or FEMALE");

            // Validate height
            if (Height < 0)
                AddToErrors(nameof(Height), "Height cannot be less than 0");
        }

        public User CreateUser()
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            User user = new()
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = BCrypt.Net.BCrypt.HashPassword(Password),
                Gender = Gender,
                Height = Height,
                DateOfBirth = DateOfBirth,
                DateCreated = now,
                DateModified = now
            };

            return user;
        }
        private void AddToErrors(string key, string message)
        {
            if (Errors.ContainsKey(key))
                Errors[key].Add(message);
            else
                Errors[key] = new List<string>() { message };
        }

        public Dictionary<string, List<string>> GetErrors()
        {
            return Errors;
        }

    }
}
