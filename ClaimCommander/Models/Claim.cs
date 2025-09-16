// In Models/Claim.cs

using ClaimCommander.Models;
using System.Reflection.Metadata;

public class Claim
{
    public int ClaimId { get; set; }
    public DateTime SubmissionDate { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal ClaimValue { get; set; }
    public string Status { get; set; }

    // Navigation Properties
    public int LecturerId { get; set; }
    public virtual User Lecturer { get; set; }

    public int SubjectId { get; set; }
    public virtual Subject Subject { get; set; } // This was missing

    public virtual ICollection<Document> SupportingDocuments { get; set; }
}