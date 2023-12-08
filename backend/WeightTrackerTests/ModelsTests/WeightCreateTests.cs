using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightTracker.Data;
using WeightTracker.Enums;
using WeightTracker.Models.User;
using WeightTracker.Models.Weight;

namespace WeightTrackerTests.ModelsTests
{
    internal class WeightCreateTests
    {
        private readonly IWeightRepo _weightRepo;
        private readonly WeightTrackerDbContext _context;
        private readonly User _existingUser;

        public WeightCreateTests()
        {
            SQLiteContext sQLiteContext = new();
            _context = sQLiteContext.CreateSQLiteContext();
            _weightRepo = new PgWeightRepo(_context);
            _existingUser = new User()
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
            };
        }

        [OneTimeSetUp]
        public void TestSetUp()
        {
            _context.AddRange(_existingUser);

            _context.AddRange(
                new Weight()
                {
                    UserId = 1,
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

        [Test, TestCaseSource(nameof(WeightCreateValidationTestProvider))]
        public void WeightCreateValidationTest(WeightCreate weightCreate, bool expectedValidationResult, int expectedNumErrors)
        {
            bool validationResult = weightCreate.IsValid(_weightRepo, _existingUser.Id).GetAwaiter().GetResult();
            int numErrors = Helper.GetNumErrors(weightCreate.GetErrors());
            Assert.Multiple(() =>
            {
                Assert.That(validationResult, Is.EqualTo(expectedValidationResult));
                Assert.That(numErrors, Is.EqualTo(expectedNumErrors));
            });
        }

        internal static object[] WeightCreateValidationTestProvider =
        [
            new object[]
            {
                /*
                 * Test for a weight create input that has the following error(s):
                 *      - Weight is less than or equal to 0
                 *      - Date and UserId combination already exists
                 */
                new WeightCreate()
                {
                    UserWeight = 0,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "something"
                },
                false,  // Expected result from validation
                2,      // Number of Errors
            },
            new object[]
            {
                /*
                 * Test for a weight create input that has the following error(s):
                 *      - Date is greater than today's date
                 */
                new WeightCreate()
                {
                    UserWeight = 72,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    Description = "something"
                },
                false,  // Expected result from validation
                1,      // Number of Errors
            },
            new object[]
            {
                /*
                 * Test for a weight create input that succeeds
                 */
                new WeightCreate()
                {
                    UserWeight = 72,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                    Description = "something"
                },
                true,   // Expected result from validation
                0,      // Number of Errors
            },
        ];
    }
}
