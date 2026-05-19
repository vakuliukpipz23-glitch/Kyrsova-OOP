using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kyrsova_OOP.Services
{
    public class DailyReminderHostedService : BackgroundService
    {
        private static readonly string[] TimeFormats =
        {
            "h\\:mm",
            "hh\\:mm",
            "h\\:mm\\:ss",
            "hh\\:mm\\:ss"
        };

        private readonly IEmailSender emailSender;
        private readonly HabitReminderService reminderService;
        private readonly SmtpOptions options;
        private readonly ILogger<DailyReminderHostedService> logger;

        public DailyReminderHostedService(
            IEmailSender emailSender,
            HabitReminderService reminderService,
            IOptions<SmtpOptions> options,
            ILogger<DailyReminderHostedService> logger)
        {
            this.emailSender = emailSender;
            this.reminderService = reminderService;
            this.options = options.Value;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timeZone = TimeZoneResolver.Resolve(options.TimeZoneId);
            var dailyTime = ParseDailyTime(options.DailyTime);

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun(timeZone, dailyTime);

                if (delay > TimeSpan.Zero)
                {
                    logger.LogInformation("Next reminder in {Delay}.", delay);
                    await Task.Delay(delay, stoppingToken);
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    if (reminderService.TryBuildReminder(out var subject, out var body))
                    {
                        await emailSender.SendAsync(subject, body, stoppingToken);
                        logger.LogInformation("Daily reminder email sent.");
                    }
                    else
                    {
                        logger.LogInformation("No incomplete habits, reminder skipped.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send daily reminder email.");
                }
            }
        }

        private static TimeSpan ParseDailyTime(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                TimeSpan.TryParseExact(value, TimeFormats, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return new TimeSpan(20, 0, 0);
        }

        private static TimeSpan GetDelayUntilNextRun(TimeZoneInfo timeZone, TimeSpan dailyTime)
        {
            var nowUtc = DateTimeOffset.UtcNow;
            var localNow = TimeZoneInfo.ConvertTime(nowUtc, timeZone);

            var nextLocal = new DateTimeOffset(
                localNow.Year,
                localNow.Month,
                localNow.Day,
                dailyTime.Hours,
                dailyTime.Minutes,
                dailyTime.Seconds,
                localNow.Offset);

            if (localNow >= nextLocal)
            {
                nextLocal = nextLocal.AddDays(1);
            }

            return nextLocal - localNow;
        }
    }
}
