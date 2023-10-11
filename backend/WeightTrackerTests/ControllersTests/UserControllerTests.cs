using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeightTracker.Controllers;
using WeightTracker.Data;
using WeightTracker.Enums;
using WeightTracker.Models;

namespace WeightTrackerTests.ControllersTests
{
    internal class UserControllerTests
    {
        private UserController _userController;
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly ILogger<UserController> _logger;

        public UserControllerTests()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _logger = Mock.Of<ILogger<UserController>>();
            _userController = new UserController(_logger, _mockUserRepo.Object);
        }

        [Test, TestCaseSource(nameof(UserControllerCreateUserTestProvider))]
        public void CreateUserTest(UserCreate user, bool emailExists, bool createUserResult, int expectedStatusCode)
        {
            _mockUserRepo.Setup(repo => repo.EmailExists(user.Email)).Returns(Task.FromResult(emailExists));
            _mockUserRepo.Setup(repo => repo.Add(It.IsAny<User>())).Returns(Task.FromResult(createUserResult));
            var result = _userController.CreateUser(user).GetAwaiter().GetResult() as ObjectResult;
           
            if (result == null)
                Assert.Fail("Null result when adding a new user");

            Assert.That(result.StatusCode, Is.EqualTo(expectedStatusCode));
        }

        internal static object[] UserControllerCreateUserTestProvider =
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
                false, // Create user result
                400,   // Expected Status Code
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
                false, // Create user result
                400,   // Expected Status Code
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
                true, // Email doesn't exist
                false, // Create user result
                400,   // Expected Status Code
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
                false, // Email doesn't exist
                false, // Create user result
                500,   // Expected Status Code
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
                false, // Email doesn't exist
                true, // Create user result
                201,   // Expected Status Code
            }
        };
    }
}
