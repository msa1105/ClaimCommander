using ClaimCommander.Models;
using ClaimCommander.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks; // Added for async operations

namespace ClaimCommander.Controllers
{
    public class LecturerController : Controller
    {
        private readonly IClaimStorageService _claimStorage;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileEncryptionService _fileEncryptionService;

        private static readonly Dictionary<string, decimal> SubjectRates = new Dictionary<string, decimal>
        {
            { "Math", 250.00m }, { "English", 220.00m }, { "Science", 275.00m },
            { "History", 210.00m }, { "Art", 280.00m }
        };

        public LecturerController(
            IClaimStorageService claimStorage,
            IWebHostEnvironment webHostEnvironment,
            IFileEncryptionService fileEncryptionService)
        {
            _claimStorage = claimStorage;
            _webHostEnvironment = webHostEnvironment;
            _fileEncryptionService = fileEncryptionService;
        }

        public IActionResult SubmitClaim()
        {
            var model = new NewClaimViewModel { Subjects = SubjectRates.Keys.ToList() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Changed to async to support the encryption service
        public async Task<IActionResult> SubmitClaim(NewClaimViewModel model)
        {
            model.Subjects = SubjectRates.Keys.ToList();
            if (!ModelState.IsValid) return View(model);

            if (!SubjectRates.TryGetValue(model.Subject, out var rate))
            {
                ModelState.AddModelError("Subject", "Invalid subject selected.");
                return View(model);
            }

            var newClaim = new Claim
            {
                LecturerName = model.LecturerName,
                HoursWorked = (decimal)model.HoursWorked,
                HourlyRate = rate,
                SubmissionDate = DateTime.UtcNow,
                Status = "Pending",
                Notes = model.Notes
            };

            // --- ADJUSTED FILE ENCRYPTION LOGIC ---
            if (model.DocumentFile != null && model.DocumentFile.Length > 0)
            {
                // 1. Define the path where files will be saved
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                // 2. Call the service to encrypt and save the file
                string encryptedFilePath = await _fileEncryptionService.EncryptAndSaveFileAsync(model.DocumentFile, uploadsFolder);

                // 3. Create the document info and store the relative path
                var documentInfo = new DocumentInfo
                {
                    FileName = model.DocumentFile.FileName,
                    FileSize = model.DocumentFile.Length,
                    UploadDate = DateTime.UtcNow,
                    // Store a web-accessible relative path
                    EncryptedFilePath = "/uploads/" + Path.GetFileName(encryptedFilePath)
                };
                newClaim.Documents.Add(documentInfo);
            }

            _claimStorage.AddClaim(newClaim);
            TempData["SuccessMessage"] = "Your claim has been submitted successfully!";
            return RedirectToAction("ViewClaims");
        }

        public IActionResult ViewClaims()
        {
            var allClaims = _claimStorage.GetAllClaims();
            var viewModel = new LecturerDashboardViewModel
            {
                AllClaims = allClaims,
                TotalHoursClaimed = allClaims.Sum(c => c.HoursWorked),
                TotalAmountClaimed = allClaims.Sum(c => c.ClaimValue),
                PendingClaimsCount = allClaims.Count(c => c.Status == "Pending" || c.Status == "CoordinatorApproved")
            };
            return View(viewModel);
        }
    }
}