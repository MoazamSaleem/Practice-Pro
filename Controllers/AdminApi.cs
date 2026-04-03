using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Pro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private async Task<IdentityUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

        private async Task<bool> HasPrimaryAdminAccess()
        {
            var user = await GetCurrentUserAsync();
            return user != null && AppAccess.IsPrimaryAdmin(user.Email);
        }

        // ✅ Get all users with status and role
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!await HasPrimaryAdminAccess())
                return Forbid();

            var dynamicAdminIds = await _db.UserRolesInfo
                .Where(r => r.RoleName == "Admin")
                .Select(r => r.UserId)
                .ToListAsync();
            var dynamicAdminIdSet = new HashSet<string>(dynamicAdminIds);

            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var result = users
                .Select(u =>
                {
                    var isPrimaryAdmin = AppAccess.IsPrimaryAdmin(u.Email);
                    return new
                    {
                        u.Id,
                        u.Email,
                        u.UserName,
                        u.LockoutEnd,
                        Status = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow ? "Blocked" : "Active",
                        IsPrimaryAdmin = isPrimaryAdmin,
                        IsAdmin = isPrimaryAdmin || dynamicAdminIdSet.Contains(u.Id)
                    };
                })
                .OrderByDescending(u => u.IsPrimaryAdmin)
                .ThenByDescending(u => u.IsAdmin)
                .ThenBy(u => u.Email)
                .ToList();

            return new JsonResult(result);
        }

        // ✅ Get total user count
        [HttpGet]
        public async Task<IActionResult> GetTotalUsers()
        {
            if (!await HasPrimaryAdminAccess())
                return Forbid();

            var total = await _userManager.Users.CountAsync();
            return Ok(new { total });
        }

        // ✅ Block user
        [HttpPut]
        public async Task<IActionResult> BlockUser(string id)
        {
            if (!await HasPrimaryAdminAccess())
                return Forbid();

            var currentUser = await GetCurrentUserAsync();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            if (AppAccess.IsPrimaryAdmin(user.Email))
                return BadRequest(new { message = "The primary admin account cannot be blocked." });

            if (currentUser != null && user.Id == currentUser.Id)
                return BadRequest(new { message = "You cannot block your own account." });

            // Block using LockoutEnd
            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);

            return Ok(new { message = "User blocked and will be logged out immediately." });
        }

        // ✅ Unblock user
        [HttpPut]
        public async Task<IActionResult> UnblockUser(string id)
        {
            if (!await HasPrimaryAdminAccess())
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
            if (!await HasPrimaryAdminAccess())
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (AppAccess.IsPrimaryAdmin(user.Email))
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
            if (!await HasPrimaryAdminAccess())
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (AppAccess.IsPrimaryAdmin(user.Email))
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
            if (!await HasPrimaryAdminAccess())
                return Forbid();

            var dynamicAdminIds = await _db.UserRolesInfo
                .Where(r => r.RoleName == "Admin")
                .Select(r => r.UserId)
                .ToListAsync();
            var dynamicAdminIdSet = new HashSet<string>(dynamicAdminIds);
            var now = DateTime.UtcNow;

            var users = await (
                from user in _userManager.Users
                join activity in _db.UserActivities on user.Id equals activity.UserId into activityGroup
                from activity in activityGroup.DefaultIfEmpty()
                select new
                {
                    user.Id,
                    user.Email,
                    user.LockoutEnd,
                    CurrentPage = activity != null ? activity.CurrentPage : null,
                    LastLogin = activity != null ? (DateTime?)activity.LastLogin : null,
                    LoginCountThisMonth = activity != null ? activity.LoginCountThisMonth : 0,
                    LastActivityTime = activity != null ? activity.LastActivityTime : null
                })
                .ToListAsync();

            var result = users
                .Select(user =>
                {
                    var isPrimaryAdmin = AppAccess.IsPrimaryAdmin(user.Email);
                    var isBlocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;

                    return new
                    {
                        UserId = user.Id,
                        UserEmail = user.Email,
                        user.CurrentPage,
                        user.LastLogin,
                        user.LoginCountThisMonth,
                        user.LastActivityTime,
                        IsPrimaryAdmin = isPrimaryAdmin,
                        IsAdmin = isPrimaryAdmin || dynamicAdminIdSet.Contains(user.Id),
                        IsBlocked = isBlocked,
                        IsActive = !isBlocked && user.LastActivityTime != null && user.LastActivityTime > now.AddMinutes(-5)
                    };
                })
                .OrderByDescending(user => user.IsActive)
                .ThenByDescending(user => user.LastActivityTime)
                .ThenBy(user => user.UserEmail)
                .ToList();

            return new JsonResult(result);
        }
    }
}
