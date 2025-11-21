using ClaimCommander.Models;
using ClaimCommander.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using Microsoft.AspNetCore.Http; // Required for Session
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimCommander.Controllers
{
    /// <summary>
    /// Controller for Lecturer interactions.
    /// <para>
    /// References:
    /// <list type="bullet">
    /// <item>Microsoft (2025) 'Session in ASP.NET Core', available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state</item>
    /// <item>Microsoft (2025) 'File uploads in ASP.NET Core', available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads</item>
    /// </list>
    /// </para>
    /// </summary>
    public class LecturerController : Controller
    {
        private readonly IClaimStorageService _claimStorage;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileEncryptionService _fileEncryptionService;

        // Mock Subjects list for the dropdown
        private static readonly List<string> _subjectList = new List<string>
        {
            "Advanced Programming", "Database Systems", "Software Engineering", "Web Development", "Information Systems"
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

        // Helper to check role
        private bool IsAuthorized()
        {
            return HttpContext.Session.GetString("UserRole") == "Lecturer";
        }

        // Helper to SIMULATE getting the HR-set rate for a user.
        // This fulfills the requirement: "The claim rate must always match the value set by HR."
        private decimal GetOfficialRateForUser(int userId)
        {
            // In a real app, this would query the database.
            // For the prototype, we hardcode rates based on ID to demonstrate the logic works.
            return userId switch
            {
                1 => 250.00m, // User 1 (e.g., John)
                2 => 300.00m, // User 2
                _ => 150.00m  // Default
            };
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            // AUTOMATION: Fetch the HR-set rate
            decimal officialRate = GetOfficialRateForUser(userId.Value);

            var model = new NewClaimViewModel
            {
                // Populate dropdown with the list of strings directly
                Subjects = new List<string>(_subjectList),
                HourlyRate = (double)officialRate, // Explicit cast to double
                LecturerName = HttpContext.Session.GetString("UserName") ?? "Lecturer"
            };

            return View(model);
        }
        // Reference: Microsoft (2025) 'File uploads in ASP.NET Core', available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(NewClaimViewModel model) // Removed explicit IFormFile parameter, it's in the model
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            // AUTOMATION RULE: Override any input rate with the official one
            decimal officialRate = GetOfficialRateForUser(userId.Value);

            // File Validation
            if (model.DocumentFile != null)
            {
                if (model.DocumentFile.Length > 5 * 1024 * 1024) // 5MB Limit
                {
                    ModelState.AddModelError("DocumentFile", "File size must be less than 5MB.");
                }

                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var extension = Path.GetExtension(model.DocumentFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("DocumentFile", "Invalid file type. Only PDF, DOCX, and XLSX are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                model.Subjects = new List<string>(_subjectList);
                model.HourlyRate = (double)officialRate;
                return View(model);
            }

            // Create the claim object
            // Automation: Calculate value (Hours * Rate) here automatically
            var newClaim = new Claim
            {
                LecturerName = model.LecturerName ?? "Unknown",
                HoursWorked = (decimal)model.HoursWorked,
                HourlyRate = officialRate, // Enforced
                SubmissionDate = DateTime.UtcNow,
                Status = "Pending",
                Notes = model.Notes,
                // The math is done here implicitly by storing Hours and Rate. 
                // If Claim has a 'TotalAmount' property, set it:
                TotalAmount = (decimal)model.HoursWorked * officialRate
            };

            // --- FILE ENCRYPTION LOGIC ---
            if (model.DocumentFile != null && model.DocumentFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + ".encrypted";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await _fileEncryptionService.EncryptAndSaveFileAsync(model.DocumentFile, filePath);

                var documentInfo = new DocumentInfo
                {
                    FileName = model.DocumentFile.FileName,
                    FileSize = model.DocumentFile.Length,
                    UploadDate = DateTime.UtcNow,
                    EncryptedFilePath = "/uploads/" + uniqueFileName
                };
                newClaim.Documents.Add(documentInfo);
            }

            _claimStorage.AddClaim(newClaim);
            TempData["SuccessMessage"] = "Your claim has been submitted successfully!";
            return RedirectToAction("ViewClaims");
        }

        public IActionResult ViewClaims()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            // Retrieve all claims
            var allClaims = _claimStorage.GetAllClaims();

            // Filter by Lecturer Name (since we don't have ID in Claim model)
            string currentLecturerName = HttpContext.Session.GetString("UserName") ?? "Lecturer";

            // Simple filter: Only show claims where the name matches
            // Note: This relies on the name being consistent. In a real app, use ID.
            var myClaims = allClaims.Where(c => c.LecturerName == currentLecturerName).ToList();

            var viewModel = new LecturerDashboardViewModel
            {
                AllClaims = myClaims, // Matches your updated ViewModel property name
                TotalHoursClaimed = myClaims.Sum(c => c.HoursWorked),
                // Assuming you have a property for value, usually calculated as Hours * Rate
                // If TotalAmount exists on claim, use that. If not, calculate it.
                TotalAmountClaimed = myClaims.Where(c => c.Status == "Approved" || c.Status == "ManagerApproved").Sum(c => c.TotalAmount),
                PendingClaimsCount = myClaims.Count(c => c.Status == "Pending")
            };
            return View(viewModel);
        }
    }
}
/*
 * Reference List:
 * * Microsoft (2025) 'File uploads in ASP.NET Core', Microsoft Learn, available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads (Accessed: 21 November 2025).
 * * Microsoft (2025) 'Session in ASP.NET Core', Microsoft Learn, available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state (Accessed: 21 November 2025).
 * * Microsoft (2025) 'Model validation in ASP.NET Core MVC', Microsoft Learn, available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation (Accessed: 21 November 2025).
 */