using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data.Common;
using WeightTracker.Authentication;
using WeightTracker.Controllers;
using WeightTracker.Controllers.QueryParameters;
using WeightTracker.Data;
using WeightTracker.Models.User;
using WeightTracker.Models.Weight;

namespace WeightTrackerTests.ControllersTests
{
    internal class WeightControllerTests
    {
        private readonly WeightController _weightController;
        private readonly Mock<IWeightRepo> _mockWeightRepo;
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly ILogger<WeightController> _logger;
        private readonly DbConnection _connection;
        private readonly DbContextOptions<WeightTrackerDbContext> _contextOptions;
        private readonly User _testUser = new() {
            Id = 1,
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "email@example.com",
            Password = "password",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Gender = WeightTracker.Enums.GenderEnum.MALE,
            Height = 165,
            DateCreated = DateTime.Now,
            DateModified = DateTime.Now
        };

        public WeightControllerTests()
        {

            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<WeightTrackerDbContext>()
                .UseSqlite(_connection)
                .Options;

            _mockWeightRepo = new Mock<IWeightRepo>();
            _mockUserRepo = new Mock<IUserRepo>();
            _mockAuthService = new Mock<IAuthService>();
            _logger = Mock.Of<ILogger<WeightController>>();
            _weightController = new WeightController(_logger, _mockWeightRepo.Object, _mockUserRepo.Object, _mockAuthService.Object);
        }

        WeightTrackerDbContext CreateContext() => new(_contextOptions);

        [OneTimeTearDown]
        public void TestTearDown()
        {
            _weightController.Dispose();
            _connection.Dispose();
        }

        [Test, TestCaseSource(nameof(WeightControllerCreateWeightTestProvider))]
        public void CreateWeightTest(WeightCreate weightCreate, bool emailFound, bool weightExistsForUserAndDate, bool createWeightResult, int expectedStatusCode)
        {
            _mockAuthService.Setup(service => service.GetEmailFromClaims()).Returns(_testUser.Email);
            if (!emailFound)
                _mockUserRepo.Setup(repo => repo.GetByEmail(It.IsAny<string>())).Returns(Task.FromResult<User?>(null));
            else
                _mockUserRepo.Setup(repo => repo.GetByEmail(It.IsAny<string>())).Returns(Task.FromResult<User?>(_testUser));
            _mockWeightRepo.Setup(repo => repo.WeightExistsForUserIdAndDate(It.IsAny<int>(), It.IsAny<DateOnly>())).Returns(Task.FromResult(weightExistsForUserAndDate));
            _mockWeightRepo.Setup(repo => repo.Add(It.IsAny<Weight>())).Returns(Task.FromResult(createWeightResult));

            var result = _weightController.CreateWeight(weightCreate).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
            int resultStatusCode = (int)result.GetType().GetProperty("StatusCode").GetValue(result, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            Assert.That(resultStatusCode, Is.EqualTo(expectedStatusCode));
            
        }

        [Test, TestCaseSource(nameof(WeightControllerGetWeightbyIdTestProvider))]
        public void GetWeightByIdTest(bool emailFound, bool weightFound, int expectedStatusCode)
        {
            Weight weightTest = new()
            {
                Id = 1,
                UserId = 1,
                UserWeight = 50,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
            };

            _mockAuthService.Setup(service => service.GetEmailFromClaims()).Returns(_testUser.Email);
            if (!emailFound)
            {
                _mockUserRepo.Setup(repo => repo.GetByEmail(It.IsAny<string>())).Returns(Task.FromResult<User?>(null));
                var result = _weightController.GetWeightById(weightTest.Id).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
                int resultStatusCode = (int)result.GetType().GetProperty("StatusCode").GetValue(result, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                Assert.That(resultStatusCode, Is.EqualTo(expectedStatusCode));
            }
            else
                _mockUserRepo.Setup(repo => repo.GetByEmail(It.IsAny<string>())).Returns(Task.FromResult<User?>(_testUser));

            if (!weightFound)
            {
                _mockWeightRepo.Setup(repo => repo.GetById(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult<Weight?>(null));
                var result = _weightController.GetWeightById(weightTest.Id).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
                int resultStatusCode = (int)result.GetType().GetProperty("StatusCode").GetValue(result, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                Assert.That(resultStatusCode, Is.EqualTo(expectedStatusCode));
            }
            else
            {
                _mockWeightRepo.Setup(repo => repo.GetById(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult<Weight?>(weightTest));
                var result = _weightController.GetWeightById(weightTest.Id).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
                int resultStatusCode = (int)result.GetType().GetProperty("StatusCode").GetValue(result, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                Assert.That(resultStatusCode, Is.EqualTo(expectedStatusCode));
            }
        }

        [Test, TestCaseSource(nameof(WeightControllerGetWeightsTestProvider))]
        public void GetWeightsTest(GetWeightsQueryParameters queryParameters, bool emailFound, bool userHasWeights, int expectedStatusCode)
        {
            _mockAuthService.Setup(service => service.GetEmailFromClaims()).Returns(_testUser.Email);
            if (!emailFound)
            {
                _mockUserRepo.Setup(repo => repo.GetByEmail(It.IsAny<string>())).Returns(Task.FromResult<User?>(null));
                var result = _weightController.GetWeights(queryParameters).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
                int resultStatusCode = (int)result.GetType().GetProperty("StatusCode").GetValue(result, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                Assert.That(resultStatusCode, Is.EqualTo(expectedStatusCode));
            }
            else
                _mockUserRepo.Setup(repo => repo.GetByEmail(It.IsAny<string>())).Returns(Task.FromResult<User?>(_testUser));

            WeightTrackerDbContext context = CreateContext();
            if (context.Database.EnsureCreated())
            {
                using var viewCommand = context.Database.GetDbConnection().CreateCommand();
                viewCommand.CommandText = @"
CREATE VIEW AllResources AS
SELECT Url
FROM Weights;";
                viewCommand.ExecuteNonQuery();
            }
            IWeightRepo weightRepo = new PgWeightRepo(context);
            WeightController weightController = new(_logger, weightRepo, _mockUserRepo.Object, _mockAuthService.Object);
            weightController.ControllerContext = new ControllerContext();
            weightController.ControllerContext.HttpContext = new DefaultHttpContext();
            weightController.ControllerContext.HttpContext.Request.Path = "/Weight";

            if (userHasWeights)
            {
                context.Add(_testUser);
                context.AddRange(TestWeights);
                context.SaveChanges();
            }
            else
                _mockWeightRepo.Setup(repo => repo.GetAllByUserId(It.IsAny<int>())).Returns(new List<Weight>().AsQueryable);

            var controllerResult = weightController.GetWeights(queryParameters).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
            int controllerResultStatusCode = (int)controllerResult.GetType().GetProperty("StatusCode").GetValue(controllerResult, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            Assert.That(controllerResultStatusCode, Is.EqualTo(expectedStatusCode));
        }

        internal static List<Weight> TestWeights = [
            new Weight()
            {
                UserId = 1,
                UserWeight = 50,
                Date = DateOnly.FromDateTime(DateTime.Now),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Weight()
            {
                UserId = 1,
                UserWeight = 50,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Weight()
            {
                UserId = 1,
                UserWeight = 50,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Weight()
            {
                UserId = 1,
                UserWeight = 50,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Weight()
            {
                UserId = 1,
                UserWeight = 50,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-4)),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Weight()
            {
                UserId = 1,
                UserWeight = 50,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Weight()
            {
                UserId = 1,
                UserWeight = 50,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-6)),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Weight()
            {
                UserId = 1,
                UserWeight = 50,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Weight()
            {
                UserId = 1,
                UserWeight = 50,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-8)),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            },
        ];

        internal static object[] WeightControllerGetWeightsTestProvider = [
            new object[]
            {
                new GetWeightsQueryParameters()
                {
                    Limit = 3,
                    Offset = 0,
                },      // Query parameters
                false,  // If the User is found with the email provided
                false,  // If the user has weights
                404     // Expected status code
            },
            new object[]
            {
                new GetWeightsQueryParameters()
                {
                    Limit = 3,
                    Offset = 0,
                },      // Query parameters
                true,  // If the User is found with the email provided
                true,  // If the user has weights
                200     // Expected status code
            },
        ];

        internal static object[] WeightControllerGetWeightbyIdTestProvider = [
            new object[]
            {
                false,  // If the User is found with the email provided
                false,  // If the weight is found with the id provided
                404     // Expected status code
            },
            new object[]
            {
                true,  // If the User is found with the email provided
                false,  // If the weight is found with the id provided
                404     // Expected status code
            },
            new object[]
            {
                true,  // If the User is found with the email provided
                true,  // If the weight is found with the id provided
                200     // Expected status code
            }
        ];

        internal static object[] WeightControllerCreateWeightTestProvider =
        [
            new object[]
            {
                new WeightCreate()
                {
                    UserWeight = 0,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "something"
                },
                false,  // If the User is found with the email provided
                false,  // Weight already exists for user id and date combination
                false,  // Create weight result
                404     // Expected status code
            },
            new object[]
            {
                new WeightCreate()
                {
                    UserWeight = 0,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "something"
                },
                true,   // If the User is found with the email provided
                false,  // Weight already exists for user id and date combination
                false,  // Create weight result
                400     // Expected status code
            },
            new object[]
            {
                new WeightCreate()
                {
                    UserWeight = 0,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "something"
                },
                true,   // If the User is found with the email provided
                false,  // Weight already exists for user id and date combination
                false,  // Create weight result
                400     // Expected status code
            },
            new object[]
            {
                new WeightCreate()
                {
                    UserWeight = 154.8,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    Description = "something"
                },
                true,   // If the User is found with the email provided
                false,  // Weight already exists for user id and date combination
                false,  // Create weight result
                400     // Expected status code
            },
            new object[]
            {
                new WeightCreate()
                {
                    UserWeight = 154.8,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "something"
                },
                true,   // If the User is found with the email provided
                true,  // Weight already exists for user id and date combination
                false,  // Create weight result
                400     // Expected status code
            },
            new object[]
            {
                new WeightCreate()
                {
                    UserWeight = 154.8,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "something"
                },
                true,   // If the User is found with the email provided
                false,  // Weight already exists for user id and date combination
                true,  // Create weight result
                201     // Expected status code
            },
        ];
    }
}
