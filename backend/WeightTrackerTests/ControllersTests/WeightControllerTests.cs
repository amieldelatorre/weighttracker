using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;
using WeightTracker.Authentication;
using WeightTracker.Controllers;
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

        public WeightControllerTests()
        {
            _mockWeightRepo = new Mock<IWeightRepo>();
            _mockUserRepo = new Mock<IUserRepo>();
            _mockAuthService = new Mock<IAuthService>();
            _logger = Mock.Of<ILogger<WeightController>>();
            _weightController = new WeightController(_logger, _mockWeightRepo.Object, _mockUserRepo.Object, _mockAuthService.Object);
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
            _weightController.Dispose();
        }

        [Test, TestCaseSource(nameof(WeightControllerCreateWeightTestProvider))]
        public void CreateWeightTest(WeightCreate weightCreate, bool emailFound, bool weightExistsForUserAndDate, bool createWeightResult, int expectedStatusCode)
        {
            User testUser = new()
            {
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

            _mockAuthService.Setup(service => service.GetEmailFromClaims()).Returns(testUser.Email);
            if (!emailFound)
                _mockUserRepo.Setup(repo => repo.GetByEmail(It.IsAny<string>())).Returns(Task.FromResult<User?>(null));
            else
                _mockUserRepo.Setup(repo => repo.GetByEmail(It.IsAny<string>())).Returns(Task.FromResult<User?>(testUser));
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
