using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Practice_Pro.Models;
using System.Diagnostics;

namespace Practice_Pro.Controllers
{
    [Route("")] // This makes all actions accessible from root
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/")]
        [HttpGet("Support")]
        [HttpGet("Term&Condition")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("DashBoard")] // Maps to "/Privacy"
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("Contact")] // Maps to "/Contact"
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet("Service")] // Maps to "/Service"
        public IActionResult Service()
        {
            return View();
        }

        [HttpGet("About")] // Maps to "/About"
        public IActionResult About()
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