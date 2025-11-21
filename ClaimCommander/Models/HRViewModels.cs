using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClaimCommander.Models
{
    // ViewModel for the main HR Dashboard
    public class HRDashboardViewModel
    {
        public List<Claim> ApprovedClaims { get; set; } = new();
        public decimal TotalPayable { get; set; }
        public int ClaimCount { get; set; }
    }

    // ViewModel for Editing Lecturer Details (Rate/Dept)
    public class LecturerInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        public string Department { get; set; } = string.Empty;
    }

    // ViewModel for the Invoice View
    public class InvoiceViewModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public Claim Claim { get; set; }
    }

    // ViewModels for the Payment Report
    public class PaymentReportViewModel
    {
        public DateTime ReportDate { get; set; }
        public List<PaymentReportItem> Items { get; set; } = new();
        public decimal GrandTotal { get; set; }
    }

    public class PaymentReportItem
    {
        public string LecturerName { get; set; } = string.Empty;
        public decimal TotalHours { get; set; }
        public decimal TotalAmount { get; set; }
        public int ClaimCount { get; set; }
    }
}