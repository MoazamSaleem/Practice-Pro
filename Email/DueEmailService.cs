//using System;
//using System.Globalization;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Practice_Pro.Models;

//public class DueEmailService
//{
//    private readonly DbCalls _db;
//    private readonly EmailService _emailService;
//    private readonly ILogger<DueEmailService> _logger;
//    private readonly UserManager<IdentityUser> _userManager;

//    public DueEmailService(
//        DbCalls db,
//        EmailService emailService,
//        ILogger<DueEmailService> logger,
//        UserManager<IdentityUser> userManager)
//    {
//        _db = db;
//        _emailService = emailService;
//        _logger = logger;
//        _userManager = userManager;
//    }

//    public async Task<int> SendDueEmailsAsync(CancellationToken cancellationToken = default)
//    {
//        var clients = await _db.client.ToListAsync(cancellationToken);
//        int emailCount = 0;

//        string[] formats = { "MM/dd/yyyy", "M/d/yyyy", "MM-dd-yyyy", "yyyy-MM-dd", "dd/MM/yyyy" };
//        DateTime today = DateTime.Today;

//        foreach (var client in clients)
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            var dateString = client.AccountsD?.Trim();

//            if (!string.IsNullOrEmpty(dateString))
//            {
//                if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate) ||
//                    DateTime.TryParse(dateString, out dueDate))
//                {
//                    int daysRemaining = (dueDate - today).Days;
//                    string subject = "";
//                    string message = "";

//                    if (daysRemaining < 0)
//                    {
//                        subject = "Overdue Account Notice";
//                        message = $"Dear {{0}}, your company {client.Name} has an overdue account since {dueDate:MM/dd/yyyy}. Please take necessary action.";
//                    }
//                    else if (daysRemaining <= 30)
//                    {
//                        subject = "Upcoming Due Date - 30 Days Remaining";
//                        message = $"Dear {{0}}, your company {client.Name} has an upcoming due date in 30 days ({dueDate:MM/dd/yyyy}). Please review.";
//                    }
//                    else if (daysRemaining <= 60)
//                    {
//                        subject = "Upcoming Due Date - 60 Days Remaining";
//                        message = $"Dear {{0}}, your company {client.Name} has an upcoming due date in 60 days ({dueDate:MM/dd/yyyy}). Please review.";
//                    }

//                    if (!string.IsNullOrEmpty(subject))
//                    {
//                        var assignedUser = await _userManager.FindByIdAsync(client.UserId);
//                        if (assignedUser != null)
//                        {
//                            await _emailService.SendEmailAsync(
//                                assignedUser.Email,
//                                subject,
//                                string.Format(message, assignedUser.UserName)
//                            );
//                            emailCount++;
//                        }
//                        else
//                        {
//                            _logger.LogWarning("No user assigned to client {ClientName}. Skipping email.", client.Name);
//                        }
//                    }
//                }
//                else
//                {
//                    _logger.LogWarning("Unable to parse date for client {ClientName}: {DateString}", client.Name, dateString);
//                }
//            }
//        }

//        _logger.LogInformation("{EmailCount} due emails sent.", emailCount);
//        return emailCount;
//    }
//}