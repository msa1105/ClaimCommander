using ClaimCommander.Models;
using ClaimCommander.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClaimCommander.Controllers
{
    public class LecturerController : Controller
    {
        private readonly IClaimStorageService _claimStorage;

        // In-memory list of subjects and their rates
        private static readonly Dictionary<string, decimal> SubjectRates = new Dictionary<string, decimal>
        {
            { "Math", 250.00m },
            { "English", 220.00m },
            { "Science", 275.00m },
            { "History", 210.00m },
            { "Art", 280.00m }
        };

        public LecturerController(IClaimStorageService claimStorage)
        {
            _claimStorage = claimStorage;
        }

        public IActionResult SubmitClaim()
        {
            var model = new NewClaimViewModel
            {
                Subjects = SubjectRates.Keys.ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitClaim(NewClaimViewModel model)
        {
            model.Subjects = SubjectRates.Keys.ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!SubjectRates.TryGetValue(model.Subject, out var rate))
            {
                ModelState.AddModelError("Subject", "Invalid subject selected.");
                return View(model);
            }

            var newClaim = new Claim
            {
                LecturerName = model.LecturerName,
                HoursWorked = (decimal)model.HoursWorked, // Cast to decimal to match Claim model
                HourlyRate = rate,
                SubmissionDate = DateTime.UtcNow,
                Status = "Pending",
                Notes = model.Notes
            };

            // ** CORRECTED FILE HANDLING LOGIC **
            if (model.DocumentFile != null && model.DocumentFile.Length > 0)
            {
                // In a real app, you would save and encrypt the file here.
                // For now, we create the metadata object as required by Claim.cs.
                var documentInfo = new DocumentInfo
                {
                    FileName = Path.GetFileName(model.DocumentFile.FileName),
                    FileSize = model.DocumentFile.Length,
                    UploadDate = DateTime.UtcNow
                    // The EncryptedFilePath would be set after saving the file.
                };

                // Add the new DocumentInfo object to the Documents list
                newClaim.Documents.Add(documentInfo);
            }

            _claimStorage.AddClaim(newClaim);
            TempData["SuccessMessage"] = "Your claim has been submitted successfully!";
            return RedirectToAction("ViewClaims");
        }

        // --- THIS IS THE UPDATED METHOD ---
        public IActionResult ViewClaims()
        {
            var allClaims = _claimStorage.GetAllClaims();

            var viewModel = new LecturerDashboardViewModel
            {
                AllClaims = allClaims,
                // Calculate and add summary data
                TotalHoursClaimed = allClaims.Sum(c => c.HoursWorked),
                TotalAmountClaimed = allClaims.Sum(c => c.ClaimValue),
                PendingClaimsCount = allClaims.Count(c => c.Status == "Pending" || c.Status == "CoordinatorApproved")
            };
            return View(viewModel);
        }
    }
}