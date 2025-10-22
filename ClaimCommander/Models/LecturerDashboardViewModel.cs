using System.Collections.Generic;
using ClaimCommander.Models;

/// <summary>
/// A comprehensive model for the entire lecturer dashboard view (lecturerpage.txt).
/// </summary>
public class LecturerDashboardViewModel
{
    // For the "Claim Status" list
    public List<Claim> RecentClaims { get; set; }

    // For the "Monthly Summary" card
    public decimal TotalHoursMonth { get; set; }
    public int PendingClaimsCount { get; set; }
    public decimal ApprovedValueMonth { get; set; }

    // For the "Submit New Claim" form
    public NewClaimViewModel NewClaimForm { get; set; }
}