using Microsoft.AspNetCore.Mvc;

namespace Practice_Pro.Controllers
{
    [Route("")]
    public class ServiceController : Controller
    {
        [HttpGet("Service/AutomatedDeadlineReminder")]
        public IActionResult AutomatedDeadlineReminder()
        {
            return View();
        }
        [HttpGet("Service/DeadlineStatusVisualization")]
        public IActionResult DeadlineStatusVisualization()
        {
            return View();
        }
        [HttpGet("Service/CompaniesHouseSync")]
        public IActionResult CompaniesHouseSync()
        {
            return View();
        }
        [HttpGet("Service/WorkflowEasier")]
        public IActionResult WorkflowEasier()
        {
            return View();
        }
        [HttpGet("Service/AutoDeadlineTracking")]
        public IActionResult AutoDeadlineTracking()
        {
            return View();
        }
    }
}
