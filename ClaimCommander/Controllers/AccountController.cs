using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Manages user authentication based on loginpage.txt.
/// </summary>
public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        // Check if the form data is valid (e.g., email and password are provided)
        if (ModelState.IsValid)
        {
            // --- Mock Login Logic ---

            // Check for lecturer credentials 🔑
            if (model.Email.ToLower() == "lecturer@example.com" && model.Password == "password123")
            {
                // If credentials match, redirect to the Lecturer's dashboard.
                return RedirectToAction("Dashboard", "Lecturer");
            }
            // Check for admin credentials 🔑
            else if (model.Email.ToLower() == "admin@example.com" && model.Password == "password123")
            {
                // If credentials match, redirect to the Admin's dashboard.
                return RedirectToAction("Dashboard", "Admin");
            }
            // If credentials do not match any of our mock users
            else
            {
                // Add an error message to display on the login page.
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
        }

        // If the model state is not valid or login failed, return to the login page to display errors.
        return View(model);
    }
}