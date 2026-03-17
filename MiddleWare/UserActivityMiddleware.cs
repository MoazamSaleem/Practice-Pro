using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Practice_Pro.Models;
using System;
using System.Threading.Tasks;

public class UserActivityMiddleware
{
    private readonly RequestDelegate _next;

    public UserActivityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<IdentityUser> userManager, DbCalls db)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user != null)
            {
                var activity = await db.UserActivities.FirstOrDefaultAsync(a => a.UserId == user.Id);
                if (activity != null)
                {
                    activity.CurrentPage = context.Request.Path;
                    activity.LastActivityTime = DateTime.UtcNow;
                    db.UserActivities.Update(activity);
                    await db.SaveChangesAsync();
                }
            }
        }

        await _next(context);
    }
}
