using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Required for Session
using ClaimCommander.Models;
using ClaimCommander.Services;

namespace ClaimCommander.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If user is already logged in, redirect them to their dashboard
            if (HttpContext.Session.GetString("UserRole") != null)
            {
                return RedirectToRoleDashboard(HttpContext.Session.GetString("UserRole"));
            }
            return View();
        }
        // Reference: Microsoft (2025) 'Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core', available at: https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            // --- CREATE SESSION ---
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.FullName);

            // Redirect to the specific dashboard for their role
            return RedirectToRoleDashboard(user.Role);
        }

        // [NEW] Logout Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Completely clear session
            return RedirectToAction("Login", "Account"); // Send back to Login screen
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
 
        // Helper to determine where each role goes
        private IActionResult RedirectToRoleDashboard(string role)
        {
            switch (role)
            {
                case "HR":
                    return RedirectToAction("Dashboard", "HR");
                case "Lecturer":
                    return RedirectToAction("SubmitClaim", "Lecturer");
                case "Coordinator":
                    return RedirectToAction("Dashboard", "Coordinator");
                case "Manager":
                    return RedirectToAction("Dashboard", "Manager");
                default:
                    return RedirectToAction("Login");
            }
        }
    }
}
/*
 * Reference List:
 * * Microsoft (2025) 'Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core', Microsoft Learn, available at: https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery (Accessed: 21 November 2025).
 * * Microsoft (2025) 'Session in ASP.NET Core', Microsoft Learn, available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state (Accessed: 21 November 2025).
 * * Microsoft (2025) 'Simple authorization in ASP.NET Core', Microsoft Learn, available at: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/simple (Accessed: 21 November 2025).
 */