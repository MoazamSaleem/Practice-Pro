using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practice_Pro.Models;

namespace Practice_Pro.Controllers
{
    [Route("api/[controller]/{action?}")]
    [ApiController]
    public class HomeApi : ControllerBase
    {
        private readonly DbCalls db;
        private readonly IWebHostEnvironment env;
        private readonly EmailService emailService; // Use Concrete Class

        public HomeApi(
            DbCalls db, // Inject DbCalls
            IWebHostEnvironment env, // Inject IWebHostEnvironment
            EmailService emailService, // Use Concrete Class
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.db = db;
            this.env = env;
            this.emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendDueEmails()
        {
            var clients = db.client.ToList();
            int emailCount = 0;
            var overdueCompanies = new List<object>();

            // Retrieve the current user's email from the claims.
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return Unauthorized("User email not found.");
            }

            // Define possible date formats.
            string[] formats = { "MM/dd/yyyy", "M/d/yyyy", "MM-dd-yyyy", "yyyy-MM-dd", "dd/MM/yyyy" };

            foreach (var client in clients)
            {
                var dateString = client.AccountsD?.Trim();
                if (!string.IsNullOrEmpty(dateString))
                {
                    // Try parsing with multiple formats; if that fails, try a more forgiving parse.
                    if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate)
                        || DateTime.TryParse(dateString, out dueDate))
                    {
                        if (dueDate < DateTime.Now)
                        {
                            // Add company info to the overdue list.
                            overdueCompanies.Add(new
                            {
                                CompanyName = client.Name,
                                DueDate = dueDate.ToString("MM/dd/yyyy")
                            });

                            // Send email notification.
                            await emailService.SendEmailAsync(
                                currentUserEmail,
                                "Account Due Notice",
                                $"Dear {client.Name}, your account is overdue as of {dueDate:MM/dd/yyyy}. Please make a payment immediately."
                            );
                            emailCount++;
                        }
                    }
                }
            }

            return Ok(new
            {
                success = true,
                message = $"{emailCount} emails sent successfully.",
                overdueCompanies = overdueCompanies
            });
        }
        [HttpGet]
        [Produces("application/json")] // Ensures JSON response
        public IActionResult GetCurrentUserEmail()
        {
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                                   ?? User.Identity?.Name; // fallback if Email claim missing

            if (string.IsNullOrEmpty(currentUserEmail))
                return BadRequest(new { status = false, message = "User email not found" });

            return Ok(new { status = true, email = currentUserEmail });
        }
    }
}