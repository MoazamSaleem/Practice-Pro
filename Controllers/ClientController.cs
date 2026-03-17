using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Practice_Pro.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ClientController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult FavCompany()
        {
            return View();
        }
        public IActionResult InCompany()
        {
            return View();
        }
    }
}
