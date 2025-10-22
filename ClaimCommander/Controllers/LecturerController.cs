using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClaimCommander.Data;
using ClaimCommander.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class LecturerController : Controller
{
    private readonly ApplicationDbContext _context;

    public LecturerController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        // Get the logged-in user's ID from the session
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            // If no user is logged in, redirect to the login page
            return RedirectToAction("Login", "Account");
        }

        // Fetch claims that belong ONLY to the logged-in user
        var userClaims = await _context.Claims
                            .Where(c => c.LecturerId == userId)
                            .Include(c => c.Subject)
                            .OrderByDescending(c => c.SubmissionDate)
                            .ToListAsync();

        var viewModel = new LecturerDashboardViewModel
        {
            RecentClaims = userClaims,
            // You can calculate these values based on userClaims later
            TotalHoursMonth = userClaims.Sum(c => c.HoursWorked),
            PendingClaimsCount = userClaims.Count(c => c.Status == "Pending"),
            ApprovedValueMonth = userClaims.Where(c => c.Status == "Approved").Sum(c => c.ClaimValue),
            NewClaimForm = new NewClaimViewModel
            {
                Subjects = new SelectList(_context.Subjects, "SubjectId", "Name")
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitClaim(NewClaimViewModel newClaim, IFormFile documentFile)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (ModelState.IsValid)
        {
            var lecturer = await _context.Users.FindAsync(userId);

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

            // File Upload Logic
            if (documentFile != null && documentFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
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

            return RedirectToAction(nameof(Dashboard));
        }
        return RedirectToAction(nameof(Dashboard));
    }
}