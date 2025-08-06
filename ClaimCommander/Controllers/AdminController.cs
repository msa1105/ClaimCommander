using Microsoft.AspNetCore.Mvc;

namespace ClaimCommander.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
