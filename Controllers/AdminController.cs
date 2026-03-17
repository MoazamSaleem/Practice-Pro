using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Pro.Models;
using System.Threading.Tasks;

namespace Practice_Pro.Controllers
{
    [Authorize] // user must be logged in
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DbCalls _db;

        public AdminController(UserManager<IdentityUser> userManager, DbCalls db)
        {
            _userManager = userManager;
            _db = db;
        }

        private async Task<bool> IsAdminUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return false;

            // ✅ Super admin always has access
            if (user.Email == "awaisshahbaz480@gmail.com")
                return true;

            // ✅ Check dynamic admin role
            var role = await _db.UserRolesInfo
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.RoleName == "Admin");

            return role != null;
        }

        public async Task<IActionResult> RegisteredUsers()
        {
            if (!await IsAdminUser())
                return RedirectToAction("AccessDenied", "Account");

            return View();
        }

        public async Task<IActionResult> UsersActivities()
        {
            if (!await IsAdminUser())
                return RedirectToAction("AccessDenied", "Account");

            return View();
        }
    }
}
