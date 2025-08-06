using System.Diagnostics;
using ClaimCommander.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClaimCommander.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ILogger<LecturerController> _logger;

        public LecturerController(ILogger<LecturerController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
