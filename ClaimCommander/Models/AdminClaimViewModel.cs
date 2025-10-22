namespace ClaimCommander.Models;
/// <summary>
/// Represents a single claim card on the admin dashboard.
/// </summary>
public class AdminClaimViewModel
{
    public int ClaimId { get; set; }
    public string LecturerName { get; set; }
    public string Department { get; set; }
    public string SubjectName { get; set; }
    public string Status { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal ClaimValue { get; set; }
    public string SubmittedAgo { get; set; }
}