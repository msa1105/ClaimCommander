using ClaimCommander.Models;
using ClaimCommander.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClaimCommander.Controllers
{
    /// <summary>
    /// Controller for lecturer claims: submitting new claims and viewing existing ones.
    /// <para>
    /// References:
    /// <list type="bullet">
    /// <item>
    /// StackOverflow. (2015) ‘How to show success message in view when RedirectToAction used’. Stack Overflow. Available at: https://stackoverflow.com/questions/27886084/how-to-show-success-message-in-view-when-redirecttoaction-used (Accessed: 21 October 2025).  
    /// </item>
    /// <item>
    /// StackOverflow. (2016) ‘Dropdown list from Dictionary MVC – ASP.NET’. Stack Overflow. Available at: https://stackoverflow.com/questions/34707152/dropdown-list-from-dictionary-mvc (Accessed: 21 October 2025).  
    /// </item>
    /// <item>
    /// Microsoft. (2024) ‘Upload files in ASP.NET Core’. Microsoft Docs. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-9.0 (Accessed: 21 October 2025).  
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
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
                HoursWorked = (decimal)model.HoursWorked,
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

            TempData["SuccessMessage"] = "Your claim has been submitted successfully!";  // Using TempData to carry a message after redirect (StackOverflow 2015)
            return RedirectToAction("ViewClaims");
        }

        /// <summary>
        /// Displays the lecturer dashboard with all claims and summary data.
        /// </summary>
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
