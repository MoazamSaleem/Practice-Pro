using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practice_Pro.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Practice_Pro.ViewComponents
{
    public class AdminMenuViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminMenuViewComponent(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
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
                            isAdmin = AppAccess.IsPrimaryAdmin(user.Email);
                        }
                    }
                }
            }

            return View("Default", isAdmin);
        }
    }
}
