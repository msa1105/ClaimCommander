using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

/// <summary>
/// Controller for the admin's claim review dashboard from adminpage.txt.
/// </summary>
public class AdminController : Controller
{
    public IActionResult Dashboard()
    {
        // Create mock data that combines user, claim, and subject info.
        var mockAdminClaims = new List<AdminClaimViewModel>
        {
            new AdminClaimViewModel { ClaimId = 1, LecturerName = "Dr. Sarah Mitchell", Department = "Mathematics Department", SubjectName = "Advanced Calculus", Status = "Pending", HoursWorked = 24, ClaimValue = 7200.00m, SubmittedAgo = "2 days ago" },
            new AdminClaimViewModel { ClaimId = 2, LecturerName = "Prof. James Kowalski", Department = "Physics Department", SubjectName = "Quantum Physics", Status = "Approved", HoursWorked = 18, ClaimValue = 5400.00m, SubmittedAgo = "1 week ago" },
            new AdminClaimViewModel { ClaimId = 3, LecturerName = "Dr. Lisa Martinez", Department = "Chemistry Department", SubjectName = "Organic Chemistry", Status = "Pending", HoursWorked = 30, ClaimValue = 9000.00m, SubmittedAgo = "5 days ago" },
            new AdminClaimViewModel { ClaimId = 4, LecturerName = "Dr. Robert Thompson", Department = "Biology Department", SubjectName = "Molecular Biology", Status = "Denied", HoursWorked = 22, ClaimValue = 6600.00m, SubmittedAgo = "2 weeks ago" }
        };

        return View(mockAdminClaims);
    }
}