using Microsoft.AspNetCore.Mvc;
using ClaimCommander.Data;
using ClaimCommander.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var pendingClaims = await _context.Claims
            .Where(c => c.Status == "Pending")
            .Include(c => c.Lecturer) // Get lecturer details
            .Include(c => c.Subject)  // Get subject details
            .Select(c => new AdminClaimViewModel
            {
                ClaimId = c.ClaimId,
                LecturerName = c.Lecturer.FullName,
                Department = c.Lecturer.Department,
                SubjectName = c.Subject.Name,
                Status = c.Status,
                HoursWorked = c.HoursWorked,
                ClaimValue = c.ClaimValue,
                SubmittedAgo = "X days ago" // This can be calculated
            }).ToListAsync();

        return View(pendingClaims);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveClaim(int claimId)
    {
        var claim = await _context.Claims.FindAsync(claimId);
        if (claim != null)
        {
            claim.Status = "Approved";
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectClaim(int claimId)
    {
        var claim = await _context.Claims.FindAsync(claimId);
        if (claim != null)
        {
            claim.Status = "Rejected";
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Dashboard));
    }
}