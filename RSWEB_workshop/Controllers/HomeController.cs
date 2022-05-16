using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSWEB_workshop.Data;
using RSWEB_workshop.Models;
using System.Diagnostics;

namespace RSWEB_workshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RSWEB_workshopContext _context;

        public HomeController(RSWEB_workshopContext context)
        {
            _context = context;
        }
        public HomeController(ILogger<HomeController> logger)
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
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser()
        {
            return View();
        }
    }
}