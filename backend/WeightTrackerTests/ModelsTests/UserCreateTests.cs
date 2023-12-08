using Microsoft.Extensions.Logging;
using Moq;
using WeightTracker.Data;
using WeightTracker.Enums;
using WeightTracker.Models.User;

namespace WeightTrackerTests.ModelsTests
{
    public class UserCreateTests
    {
        private readonly IUserRepo _userRepo;
        private readonly WeightTrackerDbContext _context;
        public UserCreateTests()
        {
            SQLiteContext sQLiteContext = new();
            _context = sQLiteContext.CreateSQLiteContext();
            _userRepo = new PgUserRepo(_context);
        }

        [OneTimeSetUp]
        public void TestSetUp()
        {
            _context.AddRange(
                new User() 
                { 
                    FirstName = "James",
                    LastName = "Smith",
                    Email = "james.smith@example.com",
                    Password = "password",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-9132)),
                    Gender = GenderEnum.MALE,
                    Height = 165,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                }
            );

            _context.SaveChanges();
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
            _context.Dispose();
        }

        [Test, TestCaseSource(nameof(UserCreateValidationTestProvider))]
        public void UserCreateValidationTest(UserCreate user, bool expectedValidationResult, int expectedNumErrors)
        {
            bool validationResult = user.IsValid(_userRepo).GetAwaiter().GetResult();
            int numErrors = Helper.GetNumErrors(user.GetErrors());

            Assert.Multiple(() =>
            {
                Assert.That(validationResult, Is.EqualTo(expectedValidationResult));
                Assert.That(numErrors, Is.EqualTo(expectedNumErrors));
            });
        }

        internal static object[] UserCreateValidationTestProvider =
        {
            new object[]
            {
                /*
                 * Test for a user create input that has the following error(s):
                 *      - Null or empty FirstName, LastName, Email
                 *      - Password that does not meet the minimum length of 8
                 *      - Date of Birth greater than today's date
                 *      - A Gender that does not fit in the GenderEnum
                 *      - A Height that is less than 0
                 */
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
                false, // Expected result from validation
                7      // Number of Errors
            },
            new object[]
            {
                /*
                 * Test for a user create input that has the following error(s):
                 *      - Password that does not meet the minimum length of 8
                 */
                new UserCreate()
                {
                    FirstName = "James",
                    LastName = "Smith",
                    Email = "james.smith2@example.com",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-365)),
                    Password = "abcd123",
                    Gender = GenderEnum.MALE,
                    Height = 150
                },
                false, // Expected result from validation
                1      // Number of Errors
            },
            new object[]
            {
                /*
                 * Test for a user create input that has the following error(s):
                 *      - Email already exists
                 */
                new UserCreate()
                {
                    FirstName = "James",
                    LastName = "Smith",
                    Email = "james.smith@example.com",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-365)),
                    Password = "abcd1234",
                    Gender = GenderEnum.MALE,
                    Height = 150
                },
                false, // Expected result from validation
                1      // Number of Errors
            },
            new object[]
            {
                /*
                 * Test for a user create input that succeeds:
                 */
                new UserCreate()
                {
                    FirstName = "James",
                    LastName = "Smith",
                    Email = "james.smith2@example.com",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-365)),
                    Password = "abcd1234",
                    Gender = GenderEnum.MALE,
                    Height = 150
                },
                true,  // Expected result from validation
                0      // Number of Errors
            }
        };
    }
}
