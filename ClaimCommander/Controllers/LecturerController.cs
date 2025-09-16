using ClaimCommander.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

/// <summary>
/// Controller for the lecturer's views and actions from lecturerpage.txt.
/// </summary>
public class LecturerController : Controller
{
    /// <summary>
    /// Displays the main dashboard for the lecturer.
    /// </summary>
    public IActionResult Dashboard()
    {
        // --- Create Mock Data for the View ---

        // 1. Mock list of recent claims for the "Claim Status" section
        var mockClaims = new List<Claim>
        {
            new Claim { Subject = new Subject{ Name = "Advanced Mathematics - Week 12" }, SubmissionDate = DateTime.Now.AddDays(-2), HoursWorked = 24, Status = "Pending" },
            new Claim { Subject = new Subject{ Name = "Database Management - Week 11" }, SubmissionDate = DateTime.Now.AddDays(-8), HoursWorked = 18, Status = "Processing" },
            new Claim { Subject = new Subject{ Name = "Computer Science - Week 10" }, SubmissionDate = DateTime.Now.AddDays(-15), HoursWorked = 20, Status = "Approved" }
        };

        // 2. Mock list of subjects for the "New Claim" form dropdown
        var mockSubjects = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Advanced Mathematics" },
            new SelectListItem { Value = "2", Text = "Computer Science Fundamentals" },
            new SelectListItem { Value = "3", Text = "Database Management Systems" }
        };

        // 3. Assemble the main ViewModel
        var viewModel = new LecturerDashboardViewModel
        {
            RecentClaims = mockClaims,
            TotalHoursMonth = 62,
            PendingClaimsCount = 3,
            ApprovedValueMonth = 12400.00m,
            NewClaimForm = new NewClaimViewModel { Subjects = mockSubjects }
        };

        return View(viewModel);
    }
}