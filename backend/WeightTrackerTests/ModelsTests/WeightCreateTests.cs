using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightTracker.Data;
using WeightTracker.Models.Weight;

namespace WeightTrackerTests.ModelsTests
{
    internal class WeightCreateTests
    {
        private readonly Mock<IWeightRepo> _mockWeightRepo;
        public WeightCreateTests()
        {
            _mockWeightRepo = new Mock<IWeightRepo>();
        }

        [Test, TestCaseSource(nameof(WeightCreateValidationTestProvider))]
        public void WeightCreateValidationTest(WeightCreate weightCreate, bool weightExistsForUserIdAndDate, bool expectedValidationResult, int expectedNumErrors)
        {
            // Setup check for if the weight already exists f or user id and date combination
            _mockWeightRepo.Setup(repo => repo.WeightExistsForUserIdAndDate(It.IsAny<int>(), It.IsAny<DateOnly>())).Returns(Task.FromResult(weightExistsForUserIdAndDate));

            bool validationResult = weightCreate.IsValid(_mockWeightRepo.Object, It.IsAny<int>()).GetAwaiter().GetResult();
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
                new WeightCreate()
                {
                    UserWeight = 0,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "something"
                },
                true,   // Weight already exists for user id and date combination
                false,  // Expected result from validation
                2,      // Number of Errors
            },
            new object[]
            {
                new WeightCreate()
                {
                    UserWeight = 0,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    Description = "something"
                },
                true,   // Weight already exists for user id and date combination
                false,  // Expected result from validation
                3,      // Number of Errors
            },
            new object[]
            {
                new WeightCreate()
                {
                    UserWeight = 154.8,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                    Description = "something"
                },
                false,  // Weight already exists for user id and date combination
                true,   // Expected result from validation
                0,      // Number of Errors
            },
        ];
    }
}
