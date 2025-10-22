using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
namespace ClaimCommander.Models

{
    public class Claim
    {
        public int ClaimId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal ClaimValue { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; } // Added for additional notes

        // Navigation Properties
        public int LecturerId { get; set; }
        public virtual User Lecturer { get; set; }

        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }

        public virtual ICollection<Document> SupportingDocuments { get; set; } = new List<Document>();
    }
}