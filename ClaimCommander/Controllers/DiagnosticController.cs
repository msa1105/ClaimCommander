using Microsoft.AspNetCore.Mvc;
using ClaimCommander.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ClaimCommander.Controllers
{
    public class DiagnosticController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiagnosticController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<h1>Diagnostic Information</h1>");
            sb.AppendLine("<div style='background: #161b22; color: #c9d1d9; padding: 20px; font-family: monospace;'>");

            // Session info
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            sb.AppendLine($"<h3>Session Info:</h3>");
            sb.AppendLine($"<p>UserId: {userId?.ToString() ?? "NULL"}</p>");
            sb.AppendLine($"<p>UserRole: {userRole ?? "NULL"}</p>");
            sb.AppendLine("<hr/>");

            // Users
            var users = await _context.Users.ToListAsync();
            sb.AppendLine($"<h3>Users ({users.Count}):</h3>");
            foreach (var user in users)
            {
                sb.AppendLine($"<p>ID: {user.UserId}, Name: {user.FullName}, Email: {user.Email}, Role: {user.Role}, Rate: {user.HourlyRate}</p>");
            }
            sb.AppendLine("<hr/>");

            // Subjects
            var subjects = await _context.Subjects.ToListAsync();
            sb.AppendLine($"<h3>Subjects ({subjects.Count}):</h3>");
            foreach (var subject in subjects)
            {
                sb.AppendLine($"<p>ID: {subject.SubjectId}, Name: {subject.Name}</p>");
            }
            sb.AppendLine("<hr/>");

            // Claims
            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Subject)
                .ToListAsync();
            sb.AppendLine($"<h3>Claims ({claims.Count}):</h3>");
            foreach (var claim in claims)
            {
                sb.AppendLine($"<p>ID: {claim.ClaimId}, Lecturer: {claim.Lecturer?.FullName ?? "NULL"}, Subject: {claim.Subject?.Name ?? "NULL"}, " +
                    $"Hours: {claim.HoursWorked}, Value: {claim.ClaimValue}, Status: {claim.Status}, Date: {claim.SubmissionDate}</p>");
            }

            sb.AppendLine("</div>");

            return Content(sb.ToString(), "text/html");
        }
    }
}