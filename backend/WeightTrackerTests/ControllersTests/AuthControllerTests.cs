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
using WeightTracker.Enums;
using WeightTracker.Models.User;

namespace WeightTrackerTests.ControllersTests
{
    internal class AuthControllerTests
    {
        private readonly AuthController _authController;
        private readonly IUserRepo _userRepo;
        private readonly ILogger<AuthController> _logger;
        private readonly WeightTrackerDbContext _context;
        private readonly User _existingUser;

        public AuthControllerTests()
        {
            SQLiteContext sQLiteContext = new();
            _context = sQLiteContext.CreateSQLiteContext();
            _userRepo = new PgUserRepo(_context);
            _logger = Mock.Of<ILogger<AuthController>>();
            _authController = new AuthController(_logger, _userRepo);
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
            _authController.Dispose();
            _context.Dispose();
        }

        [Test, TestCaseSource(nameof(AuthControllerLoginTestProvider))]
        public void LoginTest(UserLogin userLogin, int statusCode)
        {
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
                new UserLogin()
                {
                    Email = "email@email.com",
                    Password = "password"
                },
                401     // status code expected       
            },
            new object[]
            {
                new UserLogin()
                {
                    Email = "james.smith@example.com",
                    Password = "password1"
                },
                401     // status code expected       
            },
            new object[]
            {
                new UserLogin()
                {
                    Email = "james.smith1@example.com",
                    Password = "password"
                },
                401     // status code expected       
            },
            new object[]
            {
                new UserLogin()
                {
                    Email = "james.smith@example.com",
                    Password = "password"
                },
                200     // status code expected       
            }
        ];
    }
}
