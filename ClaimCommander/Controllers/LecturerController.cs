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

    // GET: Displays the dashboard
    public async Task<IActionResult> Dashboard()
    {
        var viewModel = new LecturerDashboardViewModel
        {
            // Fetch real claims from the database for the current user (mocked for now)
            RecentClaims = await _context.Claims
                                .Include(c => c.Subject) // Include subject details
                                .OrderByDescending(c => c.SubmissionDate)
                                .ToListAsync(),

            // Monthly Summary (can be calculated later)
            TotalHoursMonth = 62,
            PendingClaimsCount = 3,
            ApprovedValueMonth = 12400,

            // Populate the dropdown for the form
            NewClaimForm = new NewClaimViewModel
            {
                Subjects = new SelectList(_context.Subjects, "SubjectId", "Name")
            }
        };

        return View(viewModel);
    }

    // POST: Handles the form submission
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitClaim(NewClaimViewModel newClaim)
    {
        if (ModelState.IsValid)
        {
            // For now, we will mock the lecturer's details
            var lecturer = _context.Users.FirstOrDefault(u => u.Role == "Lecturer"); // Find a mock lecturer
            var subject = _context.Subjects.Find(newClaim.SelectedSubjectId);

            var claim = new Claim
            {
                LecturerId = lecturer.UserId,
                SubjectId = newClaim.SelectedSubjectId,
                HoursWorked = (decimal)newClaim.HoursWorked,
                SubmissionDate = DateTime.Now,
                Status = "Pending",
                ClaimValue = (decimal)newClaim.HoursWorked * lecturer.HourlyRate // Calculate value
            };

            _context.Add(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        // If model is not valid, return to the dashboard to show errors
        // You'll need to re-populate the ViewModel here, but for now we'll keep it simple
        return RedirectToAction(nameof(Dashboard));
    }
}