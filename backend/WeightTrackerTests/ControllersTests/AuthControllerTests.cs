using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightTracker.Authentication;
using WeightTracker.Controllers;
using WeightTracker.Data;
using WeightTracker.Models.User;

namespace WeightTrackerTests.ControllersTests
{
    internal class AuthControllerTests
    {
        private readonly AuthController _authController;
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly ILogger<AuthController> _logger;

        public AuthControllerTests()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _logger = Mock.Of<ILogger<AuthController>>();
            _authController = new AuthController(_logger, _mockUserRepo.Object);
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
            _authController.Dispose();
        }

        [Test, TestCaseSource(nameof(AuthControllerLoginTestProvider))]
        public void LoginTest(bool isValidLogin, int statusCode)
        {
            UserLogin userLogin = new()
            {
                Email = "email@email.com",
                Password = "password"
            };

            _mockUserRepo.Setup(repo => repo.IsValidLogin(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(isValidLogin));

            var result = _authController.Login(userLogin).GetAwaiter().GetResult();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.
            int resultStatusCode = (int)result.GetType().GetProperty("StatusCode").GetValue(result, null);
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.That(resultStatusCode, Is.EqualTo(statusCode));
        }

        internal static object[] AuthControllerLoginTestProvider =
        [
            new object[]
            {
                false,  // isValidLogin
                401     // status code expected       
            },
            new object[]
            {
                true,  // isValidLogin
                200     // status code expected       
            }
        ];
    }
}
