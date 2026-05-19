using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kyrsova_OOP.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions options;
        private readonly ILogger<SmtpEmailSender> logger;

        public SmtpEmailSender(
            IOptions<SmtpOptions> options,
            ILogger<SmtpEmailSender> logger)
        {
            this.options = options.Value;
            this.logger = logger;
        }

        public async Task SendAsync(string subject, string body, CancellationToken cancellationToken)
        {
            if (!IsConfigured())
            {
                logger.LogWarning(
                    "SMTP settings are not configured. Missing: {Missing}. Email skipped.",
                    GetMissingFields());
                return;
            }

            var toAddress = string.IsNullOrWhiteSpace(options.ToAddress)
                ? options.FromAddress
                : options.ToAddress;
            var userName = string.IsNullOrWhiteSpace(options.UserName)
                ? options.FromAddress
                : options.UserName;

            cancellationToken.ThrowIfCancellationRequested();

            using var message = new MailMessage();
            message.From = string.IsNullOrWhiteSpace(options.FromName)
                ? new MailAddress(options.FromAddress)
                : new MailAddress(options.FromAddress, options.FromName);
            message.To.Add(new MailAddress(toAddress));
            message.Subject = subject ?? string.Empty;
            message.Body = body ?? string.Empty;
            message.IsBodyHtml = false;

            using var client = new SmtpClient(options.Host, options.Port)
            {
                EnableSsl = options.UseSsl || options.UseStartTls,
                Credentials = new NetworkCredential(userName, options.Password)
            };

            await client.SendMailAsync(message);
            logger.LogInformation("SMTP email sent to {Recipient}.", toAddress);
        }

        private bool IsConfigured()
        {
            return !string.IsNullOrWhiteSpace(options.Host)
                && options.Port > 0
                && !string.IsNullOrWhiteSpace(options.FromAddress)
                && !string.IsNullOrWhiteSpace(options.Password);
        }

        private string GetMissingFields()
        {
            var missing = new List<string>();

            if (string.IsNullOrWhiteSpace(options.Host))
            {
                missing.Add("Host");
            }

            if (options.Port <= 0)
            {
                missing.Add("Port");
            }

            if (string.IsNullOrWhiteSpace(options.FromAddress))
            {
                missing.Add("FromAddress");
            }

            if (string.IsNullOrWhiteSpace(options.Password))
            {
                missing.Add("Password");
            }

            return missing.Count == 0 ? "unknown" : string.Join(", ", missing);
        }

    }
}
