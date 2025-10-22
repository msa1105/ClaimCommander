using Microsoft.AspNetCore.Mvc;
using ClaimCommander.Data;
using ClaimCommander.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ClaimCommander.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find user in the database
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    // Store user's ID and Role in the session
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // Redirect based on role
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (user.Role == "Lecturer")
                    {
                        return RedirectToAction("Dashboard", "Lecturer");
                    }
                }

                // If user not found or role is invalid, show error
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }
    }
}