using Microsoft.Extensions.Logging;
using Moq;
using WeightTracker.Data;
using WeightTracker.Enums;
using WeightTracker.Models;

namespace WeightTrackerTests.ModelsTests
{
    public class UserCreateTests
    {
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly ILogger<UserCreateTests> _logger;
        public UserCreateTests()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _logger = Mock.Of<ILogger<UserCreateTests>>();
        }

        [Test, TestCaseSource(nameof(UserCreateValidationTestProvider))]
        public void UserCreateValidationTest(UserCreate user, bool emailExists , bool expectedValidationResult, int expectedNumErrors)
        {
            // Setup check for email exists according to parameter
            _mockUserRepo.Setup(repo => repo.EmailExists(user.Email)).Returns(emailExists);

            bool validationResult = user.IsValid(_mockUserRepo.Object);
            int numErrors = GetNumErrors(user.GetErrors());
            Assert.That(validationResult, Is.EqualTo(expectedValidationResult));
            Assert.That(numErrors, Is.EqualTo(expectedNumErrors));
        }

        internal static object[] UserCreateValidationTestProvider =
        {
            new object[]
            {
                new UserCreate()
                {
                    FirstName = "",
                    LastName = "",
                    Email = "",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    Password = "abcd",
                    Gender = (GenderEnum)3,
                    Height = -1
                },
                false, // Email doesn't exist
                false, // Expected result from validation
                7      // Number of Errors
            },
            new object[]
            {
                new UserCreate()
                {
                    FirstName = "abc",
                    LastName = "def",
                    Email = "asd",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-365)),
                    Password = "abcd123",
                    Gender = GenderEnum.MALE,
                    Height = 150
                },
                false, // Email doesn't exist
                false, // Expected result from validation
                1      // Number of Errors
            },
            new object[]
            {
                new UserCreate()
                {
                    FirstName = "abc",
                    LastName = "def",
                    Email = "asd",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-365)),
                    Password = "abcd1234",
                    Gender = GenderEnum.MALE,
                    Height = 150
                },
                true,  // Email exists
                false, // Expected result from validation
                1      // Number of Errors
            },
            new object[]
            {
                new UserCreate()
                {
                    FirstName = "abc",
                    LastName = "def",
                    Email = "asd",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-365)),
                    Password = "abcd1234",
                    Gender = GenderEnum.MALE,
                    Height = 150
                },
                false,  // Email doesn't exists
                true,  // Expected result from validation
                0      // Number of Errors
            }
        };

        private int GetNumErrors(Dictionary<string, List<string>> errors)
        {
            int numErrors = 0;
            foreach (var item in errors)
            {
                numErrors += item.Value.Count;
            }
            return numErrors;
        }
    }
}
