//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;

//public class DueEmailSenderBackgroundService : BackgroundService
//{
//    private readonly IServiceScopeFactory _scopeFactory;
//    private readonly ILogger<DueEmailSenderBackgroundService> _logger;
//    private readonly TimeSpan _delay = TimeSpan.FromHours(23); // Adjust as needed

//    public DueEmailSenderBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DueEmailSenderBackgroundService> logger)
//    {
//        _scopeFactory = scopeFactory;
//        _logger = logger;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        _logger.LogInformation("DueEmailSenderBackgroundService is starting.");

//        while (!stoppingToken.IsCancellationRequested)
//        {
//            try
//            {
//                using (var scope = _scopeFactory.CreateScope())
//                {
//                    var emailService = scope.ServiceProvider.GetRequiredService<DueEmailService>();

//                    // Send emails dynamically to all registered users
//                    await emailService.SendDueEmailsAsync(stoppingToken);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while sending due emails.");
//            }

//            await Task.Delay(_delay, stoppingToken);
//        }

//        _logger.LogInformation("DueEmailSenderBackgroundService is stopping.");
//    }
//}
