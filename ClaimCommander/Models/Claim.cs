using System.ComponentModel.DataAnnotations;

namespace ClaimCommander.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }

        [Required(ErrorMessage = "Lecturer name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string LecturerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0.1, 744, ErrorMessage = "Hours must be between 0.1 and 744 (max hours in a month)")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(1, 10000, ErrorMessage = "Hourly rate must be between 1 and 10000")]
        public decimal HourlyRate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public decimal ClaimValue => HoursWorked * HourlyRate;

        public decimal TotalAmount { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        // Status: Pending, CoordinatorApproved, ManagerApproved, Rejected
        public string Status { get; set; } = "Pending";

        public string? RejectionReason { get; set; }

        // Supporting documents
        public List<DocumentInfo> Documents { get; set; } = new();
    }

    public class DocumentInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string EncryptedFilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}