using ClaimCommander.Models;
using ClaimCommander.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;

namespace ClaimCommander.Controllers
{
    /// <summary>
    /// Controller for Human Resources functionalities.
    /// Handles lecturer management, report generation, and invoice processing.
    /// <para>
    /// References:
    /// <list type="bullet">
    /// <item>Microsoft (2025) 'Controller action return types in ASP.NET Core API', available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types</item>
    /// <item>Microsoft (2025) 'Role-based authorization in ASP.NET Core', available at: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles</item>
    /// </list>
    /// </para>
    /// </summary>
    public class HRController : Controller
    {
        private readonly IClaimStorageService _storage;
        private readonly ILecturerService _lecturerService;

        public HRController(IClaimStorageService storage, ILecturerService lecturerService)
        {
            _storage = storage;
            _lecturerService = lecturerService;
        }

        // Security Helper: Checks if the logged-in user has the 'HR' role.
        private bool IsAuthorized()
        {
            return HttpContext.Session.GetString("UserRole") == "HR";
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            var approvedClaims = _storage.GetClaimsByStatus("ManagerApproved");
            var model = new HRDashboardViewModel
            {
                ApprovedClaims = approvedClaims,
                TotalPayable = approvedClaims.Sum(c => c.ClaimValue),
                ClaimCount = approvedClaims.Count
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult ManageLecturers()
        {
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            var lecturers = _lecturerService.GetAllLecturers();
            return View(lecturers);
        }

        [HttpGet]
        public IActionResult EditLecturer(int id)
        {
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            var lecturer = _lecturerService.GetLecturer(id);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction(nameof(ManageLecturers));
            }
            return View(lecturer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLecturer(LecturerInfo lecturer)
        {
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            if (!ModelState.IsValid)
            {
                return View(lecturer);
            }

            _lecturerService.UpdateLecturer(lecturer);
            TempData["SuccessMessage"] = $"Lecturer {lecturer.Name} updated successfully.";
            return RedirectToAction(nameof(ManageLecturers));
        }

        /// <summary>
        /// Generates payment reports with optional filters.
        /// HR Requirement: Generate reports with multiple filters (status, lecturer, etc).
        /// </summary>
        [HttpGet]
        public IActionResult GeneratePaymentReport(string status, string lecturerName, DateTime? startDate, DateTime? endDate)
        {
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            var claims = _storage.GetAllClaims();

            // Apply Filters
            if (!string.IsNullOrEmpty(status))
            {
                claims = claims.Where(c => c.Status == status).ToList();
            }
            if (!string.IsNullOrEmpty(lecturerName))
            {
                claims = claims.Where(c => c.LecturerName.Contains(lecturerName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (startDate.HasValue)
            {
                claims = claims.Where(c => c.SubmissionDate >= startDate.Value).ToList();
            }
            if (endDate.HasValue)
            {
                claims = claims.Where(c => c.SubmissionDate <= endDate.Value).ToList();
            }

            // Aggregate for Report
            var reportData = claims
                .GroupBy(c => c.LecturerName)
                .Select(g => new PaymentReportItem
                {
                    LecturerName = g.Key,
                    TotalHours = g.Sum(c => c.HoursWorked),
                    TotalAmount = g.Sum(c => c.ClaimValue),
                    ClaimCount = g.Count()
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            var model = new PaymentReportViewModel
            {
                ReportDate = DateTime.Now,
                Items = reportData,
                GrandTotal = reportData.Sum(x => x.TotalAmount)
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult DownloadPaymentReport()
        {
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            var approvedClaims = _storage.GetClaimsByStatus("ManagerApproved");
            var csv = GeneratePaymentReportCSV(approvedClaims);
            var bytes = Encoding.UTF8.GetBytes(csv);

            return File(bytes, "text/csv", $"PaymentReport-{DateTime.Now:yyyyMMdd}.csv");
        }

        [HttpGet]
        public IActionResult GenerateInvoice(int claimId)
        {
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            var claim = _storage.GetClaim(claimId);
            // Assuming ManagerApproved is the final approved state before payment
            if (claim == null || claim.Status != "ManagerApproved")
            {
                TempData["ErrorMessage"] = "Claim not found or not approved.";
                return RedirectToAction(nameof(Dashboard));
            }

            var invoice = new InvoiceViewModel
            {
                InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{claimId:D4}",
                InvoiceDate = DateTime.Now,
                Claim = claim,
                DueDate = DateTime.Now.AddDays(30)
            };

            return View(invoice);
        }

        [HttpGet]
        public IActionResult DownloadInvoice(int claimId)
        {
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            var claim = _storage.GetClaim(claimId);
            if (claim == null || claim.Status != "ManagerApproved")
            {
                return NotFound();
            }

            var invoice = GenerateInvoiceContent(claim);
            var bytes = Encoding.UTF8.GetBytes(invoice);

            return File(bytes, "text/html", $"Invoice-{claimId}.html");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAsPaid(int claimId)
        {
            if (!IsAuthorized()) return RedirectToAction("AccessDenied", "Account");

            var claim = _storage.GetClaim(claimId);
            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            claim.Status = "Paid";
            _storage.UpdateClaim(claim);

            TempData["SuccessMessage"] = $"Claim #{claimId} marked as paid.";
            return RedirectToAction(nameof(Dashboard));
        }

        private string GenerateInvoiceContent(Claim claim)
        {
            var invoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{claim.ClaimId:D4}";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Invoice {invoiceNumber}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .invoice-details {{ margin-bottom: 20px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th, td {{ padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }}
        .total {{ font-weight: bold; font-size: 1.2em; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>INVOICE</h1>
        <p>Invoice Number: {invoiceNumber}</p>
        <p>Date: {DateTime.Now:dd/MM/yyyy}</p>
    </div>
    
    <div class='invoice-details'>
        <p><strong>To:</strong> {claim.LecturerName}</p>
        <p><strong>Claim ID:</strong> {claim.ClaimId}</p>
        <p><strong>Submission Date:</strong> {claim.SubmissionDate:dd/MM/yyyy}</p>
    </div>
    
    <table>
        <thead>
            <tr>
                <th>Description</th>
                <th>Hours</th>
                <th>Rate</th>
                <th>Amount</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Teaching Services</td>
                <td>{claim.HoursWorked}</td>
                <td>R {claim.HourlyRate:F2}</td>
                <td>R {claim.ClaimValue:F2}</td>
            </tr>
            <tr class='total'>
                <td colspan='3' style='text-align: right;'>TOTAL:</td>
                <td>R {claim.ClaimValue:F2}</td>
            </tr>
        </tbody>
    </table>
    
    <p style='margin-top: 40px;'>
        <strong>Payment Terms:</strong> Due within 30 days<br>
        <strong>Notes:</strong> {claim.Notes ?? "N/A"}
    </p>
</body>
</html>";
        }

        private string GeneratePaymentReportCSV(List<Claim> claims)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Lecturer Name,Claim ID,Hours Worked,Hourly Rate,Total Amount,Submission Date,Status");

            foreach (var claim in claims)
            {
                sb.AppendLine($"{claim.LecturerName},{claim.ClaimId},{claim.HoursWorked},{claim.HourlyRate},{claim.ClaimValue},{claim.SubmissionDate:yyyy-MM-dd},{claim.Status}");
            }

            sb.AppendLine();
            sb.AppendLine($"Grand Total:,,,,,{claims.Sum(c => c.ClaimValue):F2}");

            return sb.ToString();
        }
    }
}

/*
 * Reference List:
 * * Microsoft (2025) 'Controller action return types in ASP.NET Core API', Microsoft Learn, available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types (Accessed: 21 November 2025).
 * * Microsoft (2025) 'Role-based authorization in ASP.NET Core', Microsoft Learn, available at: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles (Accessed: 21 November 2025).
 * * Microsoft (2025) 'Dependency injection in ASP.NET Core', Microsoft Learn, available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection (Accessed: 21 November 2025).
 */