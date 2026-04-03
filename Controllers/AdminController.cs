using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practice_Pro.Models;
using System.Threading.Tasks;

namespace Practice_Pro.Controllers
{
    [Authorize] // user must be logged in
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task<bool> HasPrimaryAdminAccess()
        {
            var user = await _userManager.GetUserAsync(User);
            return user != null && AppAccess.IsPrimaryAdmin(user.Email);
        }

        public async Task<IActionResult> RegisteredUsers()
        {
            if (!await HasPrimaryAdminAccess())
                return RedirectToAction("AccessDenied", "Account");

            return View();
        }

        public async Task<IActionResult> UsersActivities()
        {
            if (!await HasPrimaryAdminAccess())
                return RedirectToAction("AccessDenied", "Account");

            return View();
        }
    }
}
