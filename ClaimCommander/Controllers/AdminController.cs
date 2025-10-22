using Microsoft.AspNetCore.Mvc;
using ClaimCommander.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using ClaimCommander.Models;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var claims = await _context.Claims
            .Include(c => c.Lecturer)
            .Include(c => c.Subject)
            .OrderByDescending(c => c.SubmissionDate)
            .Select(c => new AdminClaimViewModel
            {
                ClaimId = c.ClaimId,
                LecturerName = c.Lecturer.FullName,
                Department = c.Lecturer.Department,
                SubjectName = c.Subject.Name,
                Status = c.Status,
                HoursWorked = c.HoursWorked,
                ClaimValue = c.ClaimValue,
                SubmittedAgo = (DateTime.Now - c.SubmissionDate).Days + " days ago"
            }).ToListAsync();

        return View(claims);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveClaim(int claimId)
    {
        try
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim != null)
            {
                claim.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Claim ID {claimId} has been approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Error: Claim ID {claimId} could not be found.";
            }
        }
        catch (Exception ex)
        {
            // Log the exception 'ex'
            TempData["ErrorMessage"] = $"Failed to approve claim {claimId}. An error occurred.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectClaim(int claimId)
    {
        try
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim != null)
            {
                claim.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Claim ID {claimId} has been rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Error: Claim ID {claimId} could not be found.";
            }
        }
        catch (Exception ex)
        {
            // Log the exception 'ex'
            TempData["ErrorMessage"] = $"Failed to reject claim {claimId}. An error occurred.";
        }
        return RedirectToAction(nameof(Dashboard));
    }
}