using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Pro.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Practice_Pro.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize] // Protect this controller
    public class AdminApi : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DbCalls _db;

        public AdminApi(UserManager<IdentityUser> userManager, DbCalls db)
        {
            _userManager = userManager;
            _db = db;
        }

        // ✅ Helper: Check if logged-in user is Admin
        private async Task<bool> IsAdminUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return false;

            // Hardcoded super admin
            if (user.Email == "awaisshahbaz480@gmail.com")
                return true;

            // Check dynamic role from UserRoleInfo table
            var role = await _db.UserRolesInfo
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.RoleName == "Admin");

            return role != null;
        }

        // ✅ Get all users with status and role
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!await IsAdminUser())
                return Forbid();

            var users = await _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.UserName,
                    u.LockoutEnd,
                    Status = u.LockoutEnd != null ? "Blocked" : "Active",

                    // ✅ Add a clean boolean flag
                    IsAdmin = _db.UserRolesInfo
                        .Any(r => r.UserId == u.Id && r.RoleName == "Admin")
                })
                .ToListAsync();

            return new JsonResult(users);
        }

        // ✅ Get total user count
        [HttpGet]
        public async Task<IActionResult> GetTotalUsers()
        {
            if (!await IsAdminUser())
                return Forbid();

            var total = await _userManager.Users.CountAsync();
            return Ok(new { total });
        }

        // ✅ Block user
        [HttpPut]
        public async Task<IActionResult> BlockUser(string id)
        {
            if (!await IsAdminUser())
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            // Block using LockoutEnd
            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);

            return Ok(new { message = "User blocked and will be logged out immediately." });
        }

        // ✅ Unblock user
        [HttpPut]
        public async Task<IActionResult> UnblockUser(string id)
        {
            if (!await IsAdminUser())
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);

            return Ok(new { message = "User unblocked successfully." });
        }

        // ✅ Promote to Admin (adds to UserRoleInfo)
        [HttpPut]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            if (!await IsAdminUser())
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.Email == "awaisshahbaz480@gmail.com")
                return BadRequest(new { message = "This user is already the main admin." });

            var existing = await _db.UserRolesInfo
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.RoleName == "Admin");

            if (existing != null)
                return BadRequest(new { message = "User is already an admin." });

            _db.UserRolesInfo.Add(new UserRoleInfo
            {
                UserId = user.Id,
                RoleName = "Admin"
            });

            await _db.SaveChangesAsync();
            return Ok(new { message = "User promoted to Admin successfully." });
        }

        // ✅ Remove Admin (deletes from UserRoleInfo)
        [HttpPut]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            if (!await IsAdminUser())
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.Email == "awaisshahbaz480@gmail.com")
                return BadRequest(new { message = "Cannot remove super admin privileges." });

            var role = await _db.UserRolesInfo
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.RoleName == "Admin");

            if (role != null)
            {
                _db.UserRolesInfo.Remove(role);
                await _db.SaveChangesAsync();
            }

            return Ok(new { message = "Admin privileges removed successfully." });
        }
        [HttpGet]
        public async Task<IActionResult> GetUserActivities()
        {
            if (!await IsAdminUser())
                return Forbid();

            var users = await _db.UserActivities
                .Select(a => new
                {
                    a.UserId,
                    UserEmail = _userManager.Users.FirstOrDefault(u => u.Id == a.UserId).Email,
                    a.CurrentPage,
                    a.LastLogin,
                    a.LoginCountThisMonth,
                    a.LastActivityTime,
                    IsActive = a.LastActivityTime != null && a.LastActivityTime > DateTime.UtcNow.AddMinutes(-1)
                })
                .ToListAsync();

            return new JsonResult(users);
        }
    }
}
