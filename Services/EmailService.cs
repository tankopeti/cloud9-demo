using Cloud9_2.Data;
using Cloud9_2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Cloud9_2.Services
{
    public class EmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmailService> _logger;

        private readonly EmailSettings _emailSettings;

        public EmailService(
            ApplicationDbContext context,
            ILogger<EmailService> logger,
            IOptions<EmailSettings> emailOptions)
        {
            _context = context;
            _logger = logger;
            _emailSettings = emailOptions.Value;
        }

        // ===============================
        // Queue email from template
        // ===============================

        public async Task<long?> QueueByTemplateAsync(
            string templateName,
            string toEmail,
            Dictionary<string, string> placeholders,
            string? ccEmail = null,
            string? bccEmail = null,
            string? relatedEntityType = null,
            int? relatedEntityId = null,
            string? createdBy = null,
            DateTime? scheduledAt = null)
        {
            var template = await _context.EmailTemplates
                .FirstOrDefaultAsync(x => x.Name == templateName && x.IsActive);

            if (template == null)
            {
                _logger.LogWarning("Email template not found: {Template}", templateName);
                return null;
            }

            var subject = ReplacePlaceholders(template.Subject, placeholders);
            var body = ReplacePlaceholders(template.Body, placeholders);

            var email = new EmailQueue
            {
                ToEmail = toEmail,
                CcEmail = ccEmail,
                BccEmail = bccEmail,
                Subject = subject,
                Body = body,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow,
                TemplateName = templateName,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId,
                CreatedBy = createdBy,
                ScheduledAt = scheduledAt
            };

            _context.EmailQueues.Add(email);
            await _context.SaveChangesAsync();

            await LogAsync(email.Id, "Queued", toEmail, subject, "Email queued", createdBy);

            return email.Id;
        }

        // ===============================
        // Queue raw email
        // ===============================

        public async Task<long> QueueRawEmailAsync(
            string toEmail,
            string subject,
            string body,
            string? ccEmail = null,
            string? bccEmail = null,
            string? relatedEntityType = null,
            int? relatedEntityId = null,
            string? createdBy = null,
            DateTime? scheduledAt = null)
        {
            var email = new EmailQueue
            {
                ToEmail = toEmail,
                CcEmail = ccEmail,
                BccEmail = bccEmail,
                Subject = subject,
                Body = body,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId,
                CreatedBy = createdBy,
                ScheduledAt = scheduledAt
            };

            _context.EmailQueues.Add(email);
            await _context.SaveChangesAsync();

            await LogAsync(email.Id, "Queued", toEmail, subject, "Raw email queued", createdBy);

            return email.Id;
        }

        // ===============================
        // Replace placeholders
        // ===============================

private string ReplacePlaceholders(string template, Dictionary<string, string> placeholders)
{
    if (string.IsNullOrWhiteSpace(template))
        return string.Empty;

    if (placeholders == null || !placeholders.Any())
        return template;

    return Regex.Replace(template, @"\{\{(.*?)\}\}", match =>
    {
        var key = match.Groups[1].Value.Trim();

        if (placeholders.TryGetValue(key, out var value))
            return value ?? string.Empty;

        return match.Value;
    });
}

        // ===============================
        // Logging
        // ===============================

        private async Task LogAsync(
            long emailQueueId,
            string eventType,
            string? toEmail,
            string? subject,
            string? message,
            string? performedBy)
        {
            var log = new EmailLog
            {
                EmailQueueId = emailQueueId,
                EventType = eventType,
                ToEmail = toEmail,
                Subject = subject,
                Message = message,
                PerformedBy = performedBy,
                EventDate = DateTime.UtcNow
            };

            _context.EmailLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task SendEmailNowAsync(
            string toEmail,
            string subject,
            string body,
            string? ccEmail = null,
            string? bccEmail = null,
            bool isHtml = true)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(toEmail);

            if (!string.IsNullOrWhiteSpace(ccEmail))
                message.CC.Add(ccEmail);

            if (!string.IsNullOrWhiteSpace(bccEmail))
                message.Bcc.Add(bccEmail);

            using var smtp = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    _emailSettings.SmtpUser,
                    _emailSettings.SmtpPassword),
                EnableSsl = _emailSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await smtp.SendMailAsync(message);
        }
        
    }
}