using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeightTracker.Authentication;
using WeightTracker.Controllers;
using WeightTracker.Controllers.QueryParameters;
using WeightTracker.Data;
using WeightTracker.Enums;
using WeightTracker.Models.User;
using WeightTracker.Models.Weight;
using System.Security.Claims;
using Castle.Components.DictionaryAdapter.Xml;
using WeightTracker.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WeightTrackerTests.ControllersTests
{
    internal class WeightControllerTests
    {
        private readonly IWeightRepo _weightRepo;
        private readonly IUserRepo _userRepo;
        private readonly ILogger<WeightController> _logger;
        private readonly WeightTrackerDbContext _context;
        private static readonly List<string> _existingUserEmails = ["james.smith@example.com", "james.smith2@example.com"];
        private readonly List<User> _existingUser;

        public WeightControllerTests()
        {
            SQLiteContext sQLiteContext = new();
            _context = sQLiteContext.CreateSQLiteContext();

            _weightRepo = new WeightRepo(_context);
            _userRepo = new UserRepo(_context);
            _logger = Mock.Of<ILogger<WeightController>>();

            _existingUser = [
                new User()
                {
                    Id = 1,
                    FirstName = "James",
                    LastName = "Smith",
                    Email = _existingUserEmails[0],
                    Password = BCrypt.Net.BCrypt.HashPassword("password"),
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-9132)),
                    Gender = GenderEnum.MALE,
                    Height = 165,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new User()
                {
                    Id = 2,
                    FirstName = "James",
                    LastName = "Smith",
                    Email = _existingUserEmails[1],
                    Password = BCrypt.Net.BCrypt.HashPassword("password"),
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(-9132)),
                    Gender = GenderEnum.MALE,
                    Height = 165,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
            ];
        }

        [OneTimeSetUp]
        public void TestSetUp()
        {
            _context.AddRange(_existingUser);
            _context.AddRange(
                new Weight()
                {
                    Id = 1,
                    UserId = _existingUser[0].Id,
                    UserWeight = 72,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new Weight()
                {
                    Id = 2,
                    UserId = _existingUser[0].Id,
                    UserWeight = 50,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new Weight()
                {
                    Id = 3,
                    UserId = _existingUser[0].Id,
                    UserWeight = 50,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new Weight()
                {
                    Id = 4,
                    UserId = _existingUser[0].Id,
                    UserWeight = 50,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new Weight()
                {
                    Id = 5,
                    UserId = _existingUser[0].Id,
                    UserWeight = 50,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-4)),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new Weight()
                {
                    Id = 6,
                    UserId = _existingUser[0].Id,
                    UserWeight = 50,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new Weight()
                {
                    Id = 7,
                    UserId = _existingUser[0].Id,
                    UserWeight = 50,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-6)),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new Weight()
                {
                    Id = 8,
                    UserId = _existingUser[0].Id,
                    UserWeight = 50,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new Weight()
                {
                    Id = 9,
                    UserId = _existingUser[0].Id,
                    UserWeight = 50,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-8)),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                },
                new Weight()
                {
                    Id = 10,
                    UserId = _existingUser[1].Id,
                    UserWeight = 50,
                    Date = DateOnly.FromDateTime(DateTime.Now),
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

        [Test, TestCaseSource(nameof(CreateWeightTestProvider))]
        public void CreateWeightTest(WeightCreate weightCreate, string email, int expectedStatusCode)
        {
            // Setup up the authentication claim Http Context
            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            DefaultHttpContext httpContext = new();
            var claims = new[] { new Claim("Email", email) };
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims, "Basic"
                )
            );
            httpContextAccessor.HttpContext = httpContext;
            AuthService authService = new AuthService(httpContextAccessor);
            WeightController weightController = new WeightController(_logger, _weightRepo, _userRepo, authService);
            
            var result = weightController.CreateWeight(weightCreate).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
            int resultStatusCode = (int)result.GetType().GetProperty("StatusCode").GetValue(result, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            Assert.That(resultStatusCode, Is.EqualTo(expectedStatusCode));
            
        }

        [Test, TestCaseSource(nameof(GetWeightbyIdTestProvider))]
        public void GetWeightByIdTest(int weightId, string email, int expectedStatusCode)
        {
            // Setup up the authentication claim Http Context
            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            DefaultHttpContext httpContext = new();
            var claims = new[] { new Claim("Email", email) };
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims, "Basic"
                )
            );
            httpContextAccessor.HttpContext = httpContext;
            AuthService authService = new AuthService(httpContextAccessor);
            WeightController weightController = new WeightController(_logger, _weightRepo, _userRepo, authService);


            var result = weightController.GetWeightById(weightId).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
            int resultStatusCode = (int)result.GetType().GetProperty("StatusCode").GetValue(result, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.That(resultStatusCode, Is.EqualTo(expectedStatusCode));
        }

        [Test, TestCaseSource(nameof(WeightControllerGetWeightsTestProvider))]
        public void GetWeightsTest(GetWeightsQueryParameters queryParameters, string email, int expectedStatusCode, int? expectedLimit, 
            int? expectedOffset, string? expectedNext)
        {
            // Setup up the authentication claim Http Context
            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            DefaultHttpContext httpContext = new();
            var claims = new[] { new Claim("Email", email) };
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims, "Basic"
                )
            );
            httpContextAccessor.HttpContext = httpContext;
            AuthService authService = new AuthService(httpContextAccessor);
            WeightController weightController = new WeightController(_logger, _weightRepo, _userRepo, authService);

            // Setup Controller Http Context Request Path
            weightController.ControllerContext = new ControllerContext();
            weightController.ControllerContext.HttpContext = new DefaultHttpContext();
            weightController.ControllerContext.HttpContext.Request.Path = "/Weight";

            var controllerResult = weightController.GetWeights(queryParameters).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
            int controllerResultStatusCode = (int)controllerResult.Result.GetType().GetProperty("StatusCode").GetValue(controllerResult.Result, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (controllerResultStatusCode == 200)
            {
                var value = (PaginatedResult<Weight>)((ObjectResult)controllerResult.Result).Value;
                Assert.That(value.Limit, Is.EqualTo(expectedLimit));
                Assert.That(value.Offset, Is.EqualTo(expectedOffset));
                Assert.That(value.Next, Is.EqualTo(expectedNext));
            }


            Assert.That(controllerResultStatusCode, Is.EqualTo(expectedStatusCode));
        }

        [Test, TestCaseSource(nameof(WeightControllerUpdateWeightsTestProvider))]
        public void UpdateWeightsTest(WeightUpdate weightUpdate, string email, int weightId, int expectedStatusCode)
        {
            // Setup up the authentication claim Http Context
            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            DefaultHttpContext httpContext = new();
            var claims = new[] { new Claim("Email", email) };
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims, "Basic"
                )
            );
            httpContextAccessor.HttpContext = httpContext;
            AuthService authService = new AuthService(httpContextAccessor);
            WeightController weightController = new WeightController(_logger, _weightRepo, _userRepo, authService);

            // Setup Controller Http Context Request Path
            weightController.ControllerContext = new ControllerContext();
            weightController.ControllerContext.HttpContext = new DefaultHttpContext();
            weightController.ControllerContext.HttpContext.Request.Path = "/Weight";

            var controllerResult = weightController.UpdateWeight(weightId, weightUpdate).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
            int controllerResultStatusCode = (int)controllerResult.GetType().GetProperty("StatusCode").GetValue(controllerResult, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.


            Assert.That(controllerResultStatusCode, Is.EqualTo(expectedStatusCode));
        }

        internal static object[] WeightControllerUpdateWeightsTestProvider = [
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Update Endpoint that has invalid input with the following error(s):
                 *      - Date is greater than today's date
                 *      - User weight is less than or equal to 0
                 */
                new WeightUpdate()
                {
                    UserWeight = 0,
                    Date  = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
                },
                _existingUserEmails[0],
                1,      // weightId to update
                400     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Update Endpoint that has invalid input with the following error(s):
                 *      - User weight is less than or equal to 0
                 */
                new WeightUpdate()
                {
                    UserWeight = 0,
                    Date  = DateOnly.FromDateTime(DateTime.Now.AddDays(-10))
                },
                _existingUserEmails[0],
                1,      // weightId to update
                400     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Update Endpoint that has invalid input with the following error(s):
                 *      - Date is greater than today's date
                 */
                new WeightUpdate()
                {
                    UserWeight = 50,
                    Date  = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
                },
                _existingUserEmails[0],
                1,      // weightId to update
                400     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Update Endpoint that has invalid input with the following error(s):
                 *      - User and date combination is unique and overlaps with an existing entry that points to the same date
                 */
                new WeightUpdate()
                {
                    UserWeight = 80,
                    Date  = DateOnly.FromDateTime(DateTime.Now.AddDays(-2))
                },
                _existingUserEmails[0],
                1,      // weightId to update
                400     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Update Endpoint that succeeds:
                 *      - When updating the weight for the same date as the original weight entry
                 */
                new WeightUpdate()
                {
                    UserWeight = 80,
                    Date  = DateOnly.FromDateTime(DateTime.Now)
                },
                _existingUserEmails[0],
                1,      // weightId to update
                200     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Update Endpoint that succeeds:
                 *      - When updating the weight to a different date from the original weight entry, with no prior entries for that date
                 */
                new WeightUpdate()
                {
                    UserWeight = 80,
                    Date  = DateOnly.FromDateTime(DateTime.Now.AddDays(-10))
                },
                _existingUserEmails[0],
                1,      // weightId to update
                200     // Expected status code
            }
        ];

        internal static object[] WeightControllerGetWeightsTestProvider = [
            // Skipping total as it becomes modified by other tests
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that has invalid input with the following error(s):
                 *      - Email does not exist
                 */
                new GetWeightsQueryParameters()
                {
                    Limit = 3,
                    Offset = 0,
                },       // Query parameters
                "email@NotFound.com",
                404,     // Expected status code
                null,    // Expected Limit
                null,    // Expected Offset
                null     // Expected Next value
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that succeeds:
                 *      - With no query parameters
                 */
                new GetWeightsQueryParameters()
                {
                },       // Query parameters
                _existingUserEmails[0],
                200,     // Expected status code
                100,     // Expected Limit
                0,       // Expected Offset
                null     // Expected Next value
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that succeeds:
                 *      - With Limit parameter
                 */
                new GetWeightsQueryParameters()
                {
                    Limit = 3,
                },       // Query parameters
                _existingUserEmails[0],
                200,     // Expected status code
                3,       // Expected Limit
                0,       // Expected Offset
                "/Weight?limit=3&offset=3"  // Expected Next value
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that succeeds:
                 *      - With Offset parameter
                 */
                new GetWeightsQueryParameters()
                {
                    Offset = 100,
                },       // Query parameters
                _existingUserEmails[0],
                200,     // Expected status code
                100,     // Expected Limit
                100,     // Expected Offset
                null     // Expected Next value
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that succeeds:
                 *      - With Limit and Offset query parameters
                 */
                new GetWeightsQueryParameters()
                {
                    Limit = 3,
                    Offset = 0,
                },       // Query parameters
                _existingUserEmails[0],
                200,     // Expected status code
                3,       // Expected Limit
                0,       // Expected Offset
                "/Weight?limit=3&offset=3"  // Expected Next value
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that succeeds:
                 *      - With Limit and Offset query parameters
                 */
                new GetWeightsQueryParameters()
                {
                    Limit = 3,
                    Offset = 3,
                },       // Query parameters
                _existingUserEmails[0],
                200,     // Expected status code
                3,       // Expected Limit
                3,       // Expected Offset
                "/Weight?limit=3&offset=6"  // Expected Next value
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that succeeds:
                 *       - With Limit and Offset query parameters
                 */
                new GetWeightsQueryParameters()
                {
                    Limit = 100,
                    Offset = 0,
                },       // Query parameters
                _existingUserEmails[0],
                200,     // Expected status code
                100,     // Expected Limit
                0,       // Expected Offset
                null     // Expected Next value
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that succeeds:
                 *       - With Limit, Offset, DateFrom and DateTo query parameters
                 */
                new GetWeightsQueryParameters()
                {
                    Limit = 3,
                    Offset = 0,
                    DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(-9)),
                    DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                },       // Query parameters
                _existingUserEmails[0],
                200,     // Expected status code
                3,       // Expected Limit
                0,       // Expected Offset
                $"/Weight?limit=3&offset=3&datefrom={DateOnly.FromDateTime(DateTime.Now.AddDays(-9)).ToString("yyyy/MM/dd")}&dateto={DateOnly.FromDateTime(DateTime.Now.AddDays(-1)).ToString("yyyy/MM/dd")}"     // Expected Next value
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that succeeds:
                 *        - With Limit, Offset and DateFrom query parameters
                 */
                new GetWeightsQueryParameters()
                {
                    Limit = 3,
                    Offset = 0,
                    DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(-9)),
                },       // Query parameters
                _existingUserEmails[0],
                200,     // Expected status code
                3,       // Expected Limit
                0,       // Expected Offset
                $"/Weight?limit=3&offset=3&datefrom={DateOnly.FromDateTime(DateTime.Now.AddDays(-9)).ToString("yyyy/MM/dd")}"     // Expected Next value
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get All Endpoint that succeeds:
                 *        - With Limit, Offset and DateTo query parameters
                 */
                new GetWeightsQueryParameters()
                {
                    Limit = 3,
                    Offset = 0,
                    DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                },       // Query parameters
                _existingUserEmails[0],
                200,     // Expected status code
                3,       // Expected Limit
                0,       // Expected Offset
                $"/Weight?limit=3&offset=3&dateto={DateOnly.FromDateTime(DateTime.Now.AddDays(-1)).ToString("yyyy/MM/dd")}"     // Expected Next value
            },
        ];

        internal static object[] GetWeightbyIdTestProvider = [
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get by Id Endpoint that has invalid input with the following error(s):
                 *      - Weight Id cannot be found
                 *      - Email cannot be found
                 */
                500,      // Weight Id
                "email@NotFound.com",
                404     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get by Id Endpoint that has invalid input with the following error(s):
                 *      - Email cannot be found
                 */
                1,      // Weight Id
                "email@NotFound.com",
                404     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get by Id Endpoint that has invalid input with the following error(s):
                 *      - Weight Id cannot be found
                 */
                501,      // Weight Id
                _existingUserEmails[0],
                404     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get by Id Endpoint that has invalid input with the following error(s):
                 *      - Weight with Weight Id of 1 belongs to _existingUserEmails[0]
                 */
                1,      // Weight Id
                _existingUserEmails[1],
                404     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Get by Id Endpoint that succeeds
                 */
                1,      // Weight Id
                _existingUserEmails[0],
                200     // Expected status code
            },
        ];

        internal static object[] CreateWeightTestProvider = [
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Post Endpoint that has invalid input with the following error(s):
                 *      - An Email that does not exist
                 */
                new WeightCreate()
                {
                    UserWeight = 72,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                    Description = "something"
                },
                "email@NotFound.com",
                404     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Post Endpoint that has invalid input with the following error(s):
                 *      - User weight is less than or equal to 0
                 */
                new WeightCreate()
                {
                    UserWeight = 0,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                    Description = "something"
                },
                _existingUserEmails[0],
                400     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Post Endpoint that has invalid input with the following error(s):
                 *      - Date is greater than today's date
                 */
                new WeightCreate()
                {
                    UserWeight = 72,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    Description = "something"
                },
                _existingUserEmails[0],
                400     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Post Endpoint that has invalid input with the following error(s):
                 *      - User and Date combination already exists
                 */
                new WeightCreate()
                {
                    UserWeight = 72,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "something"
                },
                _existingUserEmails[0],
                400     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Post Endpoint that succeeds
                 */
                new WeightCreate()
                {
                    UserWeight = 72,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                    Description = "something"
                },
                _existingUserEmails[0],
                201     // Expected status code
            },
            new object[]
            {
                /*
                 * Test the response of the Weight Controller Post Endpoint that succeeds
                 *      - When existingUserEmails[1] creates a weight for the same day as existingUserEmails[0]
                 */
                new WeightCreate()
                {
                    UserWeight = 72,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                    Description = "something"
                },
                _existingUserEmails[1],
                201     // Expected status code
            }
        ];
    }
}
