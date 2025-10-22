using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClaimCommander.Data;
using ClaimCommander.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

public class LecturerController : Controller
{
    private readonly ApplicationDbContext _context;

    public LecturerController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Helper method to prepare the ViewModel
    private async Task<LecturerDashboardViewModel> PrepareDashboardViewModel(int userId)
    {
        var userClaims = await _context.Claims
                               .Where(c => c.LecturerId == userId)
                               .Include(c => c.Subject)
                               .OrderByDescending(c => c.SubmissionDate)
                               .ToListAsync();

        return new LecturerDashboardViewModel
        {
            RecentClaims = userClaims,
            TotalHoursMonth = userClaims.Sum(c => c.HoursWorked),
            PendingClaimsCount = userClaims.Count(c => c.Status == "Pending"),
            ApprovedValueMonth = userClaims.Where(c => c.Status == "Approved").Sum(c => c.ClaimValue),
            NewClaimForm = new NewClaimViewModel
            {
                Subjects = new SelectList(_context.Subjects, "SubjectId", "Name")
            }
        };
    }

    // GET: Displays the dashboard
    public async Task<IActionResult> Dashboard()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var viewModel = await PrepareDashboardViewModel(userId.Value);
        return View(viewModel);
    }

    // POST: Handles the form submission
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitClaim(NewClaimViewModel newClaim, IFormFile documentFile)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["ErrorMessage"] = "Your session has expired. Please log in again.";
            return RedirectToAction("Login", "Account");
        }

        // --- Model Validation Logic ---
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Failed to submit claim. Please check the form for errors.";
            // Rebuild the full view model and return the view directly to display errors
            var viewModel = await PrepareDashboardViewModel(userId.Value);
            viewModel.NewClaimForm = newClaim; // Keep the user's invalid input
            return View("Dashboard", viewModel);
        }

        // --- Database & File Logic ---
        try
        {
            var lecturer = await _context.Users.FindAsync(userId);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Could not find your user record.";
                return RedirectToAction(nameof(Dashboard));
            }

            var claim = new Claim
            {
                LecturerId = lecturer.UserId,
                SubjectId = newClaim.SelectedSubjectId,
                HoursWorked = (decimal)newClaim.HoursWorked,
                SubmissionDate = DateTime.Now,
                Status = "Pending",
                ClaimValue = (decimal)newClaim.HoursWorked * lecturer.HourlyRate
            };

            _context.Add(claim);
            await _context.SaveChangesAsync();

            if (documentFile != null && documentFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(documentFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await documentFile.CopyToAsync(stream);
                }

                var document = new Document
                {
                    ClaimId = claim.ClaimId,
                    FileName = documentFile.FileName,
                    FilePath = "/uploads/" + uniqueFileName
                };
                _context.Documents.Add(document);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Claim submitted successfully!";
            return RedirectToAction(nameof(Dashboard));
        }
        catch (Exception ex)
        {
            // In a real app, you would log the exception 'ex'
            TempData["ErrorMessage"] = $"Failed to submit claim due to a system error.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}