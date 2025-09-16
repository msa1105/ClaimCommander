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
        if (ModelState.IsValid)
        {
            // TODO: Authenticate user against a database.
            // For the prototype, we can redirect based on a dummy email.
            if (model.Email.Contains("lecturer"))
            {
                return RedirectToAction("Dashboard", "Lecturer");
            }
            else
            {
                return RedirectToAction("Dashboard", "Admin");
            }
        }
        return View(model);
    }
}