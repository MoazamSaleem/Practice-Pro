using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Practice_Pro.Models;

namespace Practice_Pro
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddDbContext<DbCalls>(o => o.UseSqlServer(Configuration.GetConnectionString("Connection")));
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<DbCalls>()
            .AddDefaultTokenProviders();


            // Register services
            services.AddScoped<EmailService>();
            //services.AddScoped<DueEmailService>();
            //services.AddHostedService<DueEmailSenderBackgroundService>();

            // Configure cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login"; // Your login path
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Match session timeout
                    options.SlidingExpiration = true; // Reset the expiration time on each request
                    options.Cookie.IsEssential = true; // Ensure the cookie is treated as essential
                    options.Cookie.HttpOnly = true; // Prevent client-side script access to the cookie
                    options.Cookie.SameSite = SameSiteMode.Strict; // Enhance security
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowAllOrigins"); // Ensure CORS is properly set up
            app.UseAuthentication();
            app.UseMiddleware<BlockUserMiddleware>();
            app.UseAuthorization();
            app.UseMiddleware<UserActivityMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                // 1. Map controllers with attribute routing (higher priority)
                endpoints.MapControllers();

                // 2. Fallback to default MVC route (for other controllers)
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
