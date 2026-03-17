using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Practice_Pro.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

public class BlockUserMiddleware
{
    private readonly RequestDelegate _next;

    public BlockUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DbCalls db, UserManager<IdentityUser> userManager)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null && user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now)
                {
                    // ❌ User is blocked → Logout immediately
                    await context.SignOutAsync();

                    context.Response.Redirect("/Account/Login?blocked=true");
                    return;
                }
            }
        }

        await _next(context);
    }
}
