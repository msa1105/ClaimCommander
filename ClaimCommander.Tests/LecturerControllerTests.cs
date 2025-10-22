using ClaimCommander.Controllers;
using ClaimCommander.Models;
using ClaimCommander.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace ClaimCommander.Tests
{
    // Helper class to create a concrete TempDataSerializer for testing
    public class TestTempDataSerializer : TempDataSerializer
    {
        public override IDictionary<string, object> Deserialize(byte[] unprotectedData)
        {
            // This is a dummy implementation that returns an empty dictionary.
            // For these tests, we don't need to actually serialize/deserialize TempData.
            return new Dictionary<string, object>();
        }

        public override byte[] Serialize(IDictionary<string, object> values)
        {
            // This is a dummy implementation that returns an empty byte array.
            return Encoding.UTF8.GetBytes("");
        }
    }

    [TestClass]
    public class LecturerControllerTests
    {
        private IClaimStorageService _storageService;
        private LecturerController _controller;

        [TestInitialize]
        public void Setup()
        {
            // This runs before each test
            _storageService = new InMemoryClaimStorageService();
            _controller = new LecturerController(_storageService);

            // --- Correctly set up TempData for testing ---
            var services = new ServiceCollection();
            services.AddDataProtection();
            services.AddLogging();
            var serviceProvider = services.BuildServiceProvider();

            var dataProtectionProvider = serviceProvider.GetRequiredService<IDataProtectionProvider>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            // Use our concrete helper class here
            var tempDataSerializer = new TestTempDataSerializer();

            var tempDataProvider = new CookieTempDataProvider(
                dataProtectionProvider,
                loggerFactory,
                Options.Create(new CookieTempDataProviderOptions()),
                tempDataSerializer // The final missing argument
            );

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                tempDataProvider
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TestMethod]
        public void SubmitClaim_Get_ReturnsViewWithSubjects()
        {
            // Act
            var result = _controller.SubmitClaim() as ViewResult;
            var model = result?.Model as NewClaimViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Subjects.Count > 0);
        }

        [TestMethod]
        public void SubmitClaim_Post_WithValidModel_RedirectsToViewClaims()
        {
            // Arrange
            var validModel = new NewClaimViewModel
            {
                LecturerName = "Test Lecturer",
                Subject = "Math",
                HoursWorked = 5
            };

            // Act
            var result = _controller.SubmitClaim(validModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ViewClaims", result.ActionName);
        }

        [TestMethod]
        public void ViewClaims_ReturnsViewModel_WithCalculatedTotals()
        {
            // Act
            var result = _controller.ViewClaims() as ViewResult;
            var model = result?.Model as LecturerDashboardViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.TotalHoursClaimed > 0);
            Assert.IsTrue(model.TotalAmountClaimed > 0);
            Assert.AreEqual(3, model.PendingClaimsCount);
        }

        [TestMethod]
        public void AddClaim_CalculatesTotalAmount_And_AssignsId()
        {
            // Arrange
            var newClaim = new Claim
            {
                HoursWorked = 10,
                HourlyRate = 200m
            };

            // Act
            int newId = _storageService.AddClaim(newClaim);

            // Assert
            Assert.IsTrue(newId > 0, "ClaimId should be assigned.");
            Assert.AreEqual(2000m, newClaim.TotalAmount, "TotalAmount was not calculated correctly.");
        }

        [TestMethod]
        public void GetAllClaims_AfterAddingClaim_ReturnsCorrectCount()
        {
            // Arrange
            int initialCount = _storageService.GetAllClaims().Count;
            var newClaim = new Claim { HoursWorked = 5, HourlyRate = 100m };

            // Act
            _storageService.AddClaim(newClaim);
            var allClaims = _storageService.GetAllClaims();

            // Assert
            Assert.AreEqual(initialCount + 1, allClaims.Count, "The claim count should increase by one.");
        }
    }
}