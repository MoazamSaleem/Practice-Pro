using FirstCoreApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Practice_App.Models;
using Practice_Pro.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Practice_App.Controllers
{
    [Route("api/[controller]/{action?}")]
    [ApiController]
    public class AccountApi : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly EmailService emailService;
        private readonly ILogger<AccountApi> logger;

        public AccountApi(UserManager<IdentityUser> userManager,
                          SignInManager<IdentityUser> signInManager,
                          EmailService emailService,
                          ILogger<AccountApi> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
            this.logger = logger;
        }

        public async Task<JsonResult> register(_sign data)
        {
            if (data?.obj == null || string.IsNullOrWhiteSpace(data.obj.email) || string.IsNullOrWhiteSpace(data.obj.password))
            {
                return new JsonResult(new { status = false, message = "Invalid signup request." });
            }

            try
            {
                var user = new IdentityUser
                {
                    UserName = data.obj.email,
                    Email = data.obj.email,
                };

                var result = await userManager.CreateAsync(user, data.obj.password);

                if (!result.Succeeded)
                {
                    return new JsonResult(new { status = false, message = "Signup failed", errors = result.Errors });
                }

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                string emailWarning = null;
                try
                {
                    await emailService.SendEmailAsync(
          user.Email,
          "Confirm Your Email - PremiumDM",
          $@"
    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; padding: 20px; border-radius: 10px;'>
        <h2 style='color: #2c3e50;'>Welcome to PremiumDM!</h2>
        <p>Hello {user.UserName},</p>
        <p>Thank you for registering with <strong>PremiumDM</strong>. To get started, please confirm your email address by clicking the button below:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{confirmationLink}' style='background-color: #28a745; color: white; padding: 12px 20px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Confirm Email</a>
        </div>
        <p>If you did not create this account, no further action is required.</p>
        <p>Thank you,<br/>The PremiumDM Team</p>
        <hr style='margin-top: 30px;'/>
        <small style='color: #777;'>This confirmation link is valid for a limited time.</small>
    </div>"
      );

                    // Send notification to admin
                    string adminEmail = "info@premiumaccountants.co.uk";
                    string emailBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px; background-color: #f9f9f9;'>
            <h2 style='color: #0000e6;'>👤 New User Registered on <strong>PremiumDM</strong></h2>
            <p>A new user has just registered on PremiumDM.</p>
            <p><strong>Email:</strong> {data.obj.email}</p>
            <p><strong>Registration Time:</strong> {DateTime.UtcNow.ToString("f")} (UTC)</p>
            <hr style='border: none; border-top: 1px solid #ddd;' />
            <p style='font-size: 12px; color: #777;'>This is an automated notification from PremiumDM.</p>
        </div>";

                    await emailService.SendEmailAsync(
                        adminEmail,
                        "👤 New Registration on PremiumDM",
                        emailBody);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "User created but email sending failed for {Email}", data.obj.email);
                    emailWarning = "Account created, but confirmation email could not be sent.";
                }

                return new JsonResult(new
                {
                    status = true,
                    userId = user.Id,
                    token = token,
                    confirmationLink = confirmationLink,
                    message = emailWarning ?? "Registration successful."
                });   
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Register failed for {Email}", data.obj.email);
                return new JsonResult(new { status = false, message = "Registration is currently unavailable. Please try again shortly." });
            }   
        }

        public async Task<JsonResult> login(_signin data)
        {
            if (data?.obj == null || string.IsNullOrWhiteSpace(data.obj.Email) || string.IsNullOrWhiteSpace(data.obj.Password))
            {
                return new JsonResult(new { status = false, error = "Email and password are required." });
            }

            try
            {
                var isPersistent = data.obj.RememberMe;
                var result = await signInManager.PasswordSignInAsync(
                    data.obj.Email,
                    data.obj.Password,
                    isPersistent,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(data.obj.Email);
                    if (user != null)
                    {
                        try
                        {
                            AuthenticationProperties authProps = new AuthenticationProperties();
                            if (data.obj.RememberMe)
                            {
                                authProps.IsPersistent = true;
                                authProps.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2);
                            }

                            // Create scope to access DB for custom claims + activity tracking.
                            using (var scope = HttpContext.RequestServices.CreateScope())
                            {
                                var db = scope.ServiceProvider.GetRequiredService<DbCalls>();

                                bool isAdmin = AppAccess.IsPrimaryAdmin(user.Email) ||
                                               await db.UserRolesInfo.AnyAsync(r => r.UserId == user.Id && r.RoleName == "Admin");

                                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("IsAdmin", isAdmin ? "true" : "false")
                };

                                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                                var principal = new ClaimsPrincipal(identity);

                                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProps);

                                var activity = await db.UserActivities.FirstOrDefaultAsync(a => a.UserId == user.Id);
                                if (activity == null)
                                {
                                    activity = new UserActivity
                                    {
                                        UserId = user.Id,
                                        LastLogin = DateTime.UtcNow,
                                        LoginCountThisMonth = 1,
                                        LastActivityTime = DateTime.UtcNow,
                                        CurrentPage = "Home"
                                    };
                                    db.UserActivities.Add(activity);
                                }
                                else
                                {
                                    var now = DateTime.UtcNow;
                                    if (activity.LastLogin.Month != now.Month || activity.LastLogin.Year != now.Year)
                                        activity.LoginCountThisMonth = 1;
                                    else
                                        activity.LoginCountThisMonth += 1;

                                    activity.LastLogin = now;
                                    activity.LastActivityTime = now;
                                    activity.CurrentPage = "Home";

                                    db.UserActivities.Update(activity);
                                }

                                await db.SaveChangesAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning(ex, "Login succeeded, but custom claims/activity update failed for {Email}", data.obj.Email);
                        }
                    }

                    return new JsonResult(new { status = true, result });
                }
                else
                {
                    return new JsonResult(new { status = false, error = "Invalid login attempt" });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Login failed for {Email}", data.obj.Email);
                return new JsonResult(new { status = false, error = "Sign-in is currently unavailable. Please try again shortly." });
            }
        }

        public async Task<IActionResult> forgot(_forgot data)
        {
            var user = await userManager.FindByEmailAsync(data.obj.Email);
            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = data.obj.Email, token = token }, Request.Scheme);
                // Send email (using your existing EmailService)
                await emailService.SendEmailAsync(
    user.Email,
    "Password Reset Request - PremiumDM",
    $@"
    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
        <h2 style='color: #2c3e50;'>PremiumDM - Password Reset Request</h2>
        <p>Hello {user.UserName},</p>
        <p>We received a request to reset your password for your PremiumDM account.</p>
        <p>To reset your password, please click the button below:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{passwordResetLink}' style='background-color: #007bff; color: white; padding: 12px 20px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Reset Password</a>
        </div>
        <p>If you didn’t request a password reset, you can safely ignore this email.</p>
        <p>Thanks,<br>The PremiumDM Team</p>
        <hr style='margin-top: 30px;'/>
        <small style='color: #777;'>This link will expire in 24 hours for your security.</small>
    </div>"
);

                return new JsonResult(new
                {
                    status = true,
                    passwordResetLink,
                    token
                });
            }
            return new JsonResult(new
            {
                status = false
            });
        }

        public async Task<IActionResult> reset(_reset data)
        {
            var user = await userManager.FindByEmailAsync(data.obj.email);
            if (user == null)
            {
                return new JsonResult(new
                {
                    status = false,
                    message = "User not found"
                });
            }

            var result = await userManager.ResetPasswordAsync(user, data.obj.Token, data.obj.password);
            if (result.Succeeded)
            {
                return new JsonResult(new
                {
                    status = true
                });
            }

            return new JsonResult(new
            {
                status = false,
                message = "Password reset failed"
            });
        }
        public class _sign
        {
            public Register obj { get; set; }
        }

        public class _signin
        {
            public Login obj { get; set; }
        }
        public class _reset
        {
            public ResetPassword obj { get; set; }
        }
        public class _forgot
        {
            public ForgetPassword obj { get; set; }
        }
    }
}
