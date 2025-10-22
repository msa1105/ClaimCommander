using ClaimCommander.Models;
using ClaimCommander.Services;

namespace ClaimCommander.Tests
{
    [TestClass]
    public class InClaimStorageServiceTests
    {
        [TestMethod]
        public void AddClaim_CalculatesTotalAmount_And_AssignsId()
        {
            // Arrange: Create the service and a new claim
            var storageService = new InMemoryClaimStorageService();
            var newClaim = new Claim
            {
                HoursWorked = 10,
                HourlyRate = 200m
            };

            // Act: Add the claim using the service
            int newId = storageService.AddClaim(newClaim);

            // Assert: Check if the ID and TotalAmount were set correctly
            Assert.IsTrue(newId > 0, "ClaimId should be assigned.");
            Assert.AreEqual(2000m, newClaim.TotalAmount, "TotalAmount was not calculated correctly.");
        }

        [TestMethod]
        public void GetAllClaims_AfterAddingClaim_ReturnsCorrectCount()
        {
            // Arrange: Create the service and get the initial count
            var storageService = new InMemoryClaimStorageService();
            // The service starts with 5 mock claims
            int initialCount = storageService.GetAllClaims().Count;
            var newClaim = new Claim { HoursWorked = 5, HourlyRate = 100m };

            // Act: Add a new claim
            storageService.AddClaim(newClaim);
            var allClaims = storageService.GetAllClaims();

            // Assert: Check if the count increased by one
            Assert.AreEqual(initialCount + 1, allClaims.Count, "The claim count should increase by one.");
        }
    }
}