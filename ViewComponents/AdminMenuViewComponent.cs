using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Pro.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Practice_Pro.ViewComponents
{
    public class AdminMenuViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DbCalls _db;

        private const string SuperAdminEmail = "awaisshahbaz480@gmail.com";

        public AdminMenuViewComponent(UserManager<IdentityUser> userManager, DbCalls db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            bool isAdmin = false;

            if (User.Identity.IsAuthenticated)
            {
                var claimsPrincipal = User as ClaimsPrincipal;

                if (claimsPrincipal != null)
                {
                    var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            if (user.Email == SuperAdminEmail)
                            {
                                isAdmin = true;
                            }
                            else
                            {
                                isAdmin = await _db.UserRolesInfo
                                    .AnyAsync(r => r.UserId == userId && r.RoleName == "Admin");
                            }
                        }
                    }
                }
            }

            return View("Default", isAdmin);
        }
    }
}
