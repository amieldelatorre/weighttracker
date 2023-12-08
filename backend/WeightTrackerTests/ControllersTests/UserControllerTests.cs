using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeightTracker.Authentication;
using WeightTracker.Controllers;
using WeightTracker.Data;
using WeightTracker.Enums;
using WeightTracker.Models.User;

namespace WeightTrackerTests.ControllersTests
{
    internal class UserControllerTests
    {
        private readonly UserController _userController;
        private readonly IUserRepo _userRepo;
        private readonly ILogger<UserController> _logger;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly WeightTrackerDbContext _context;
        private readonly User _existingUser;

        public UserControllerTests()
        {
            SQLiteContext sQLiteContext = new();
            _context = sQLiteContext.CreateSQLiteContext();
            _userRepo = new PgUserRepo(_context);
            _logger = Mock.Of<ILogger<UserController>>();
            _mockAuthService = new Mock<IAuthService>();

            _userController = new UserController(_logger, _userRepo, _mockAuthService.Object);

            _existingUser = new User()
            {
                FirstName = "James",
                LastName = "Smith",
                Email = "james.smith@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-9132)),
                Gender = GenderEnum.MALE,
                Height = 165,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };
        }

        [OneTimeSetUp]
        public void TestSetUp()
        {
            _context.AddRange(_existingUser);
            _context.SaveChanges();
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
            _context.Dispose();
        }

        [Test, TestCaseSource(nameof(CreateUserTestProvider))]
        public void CreateUserTest(UserCreate user, int expectedStatusCode)
        {
            var result = _userController.CreateUser(user).GetAwaiter().GetResult() as ObjectResult;
           
            if (result == null)
                Assert.Fail("Null result when adding a new user");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.That(result.StatusCode, Is.EqualTo(expectedStatusCode));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        internal static object[] CreateUserTestProvider =
        [
            new object[]
            {
                /*
                 * Test the response of the User Controller that has invalid input with the following error(s):
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
                400   // Expected Status Code
            },
            new object[]
            {
                /*
                 * Test the response of the User Controller that has invalid input with the following error(s):
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
                400   // Expected Status Code
            },
            new object[]
            {
                /*
                 * Test the response of the User Controller that has invalid input with the following error(s):
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
                400   // Expected Status Code
            },
            new object[]
            {
                /*
                 * Test the response of the User Controller that succeeds
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
                201   // Expected Status Code
            }
        ];
    }
}
