using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScheduleKit.Application.Common.Interfaces;

namespace ScheduleKit.Infrastructure.Services;

/// <summary>
/// SMTP email service implementation.
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendBookingConfirmationAsync(
        BookingEmailData data,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Booking Confirmed: {data.EventTypeName} with {data.HostName}";
        var body = BuildConfirmationEmailBody(data);

        await SendEmailAsync(data.GuestEmail, subject, body, data.IcsAttachment, cancellationToken);
    }

    public async Task SendBookingCancellationAsync(
        BookingEmailData data,
        string? reason,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Booking Cancelled: {data.EventTypeName}";
        var body = BuildCancellationEmailBody(data, reason);

        await SendEmailAsync(data.GuestEmail, subject, body, null, cancellationToken);
    }

    public async Task SendBookingRescheduledAsync(
        BookingEmailData data,
        DateTime oldStartTime,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Booking Rescheduled: {data.EventTypeName}";
        var body = BuildRescheduledEmailBody(data, oldStartTime);

        await SendEmailAsync(data.GuestEmail, subject, body, data.IcsAttachment, cancellationToken);
    }

    public async Task SendBookingReminderAsync(
        BookingEmailData data,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Reminder: {data.EventTypeName} with {data.HostName} is coming up";
        var body = BuildReminderEmailBody(data);

        await SendEmailAsync(data.GuestEmail, subject, body, null, cancellationToken);
    }

    public async Task SendNewBookingNotificationToHostAsync(
        BookingEmailData data,
        string hostEmail,
        CancellationToken cancellationToken = default)
    {
        var subject = $"New Booking: {data.EventTypeName} with {data.GuestName}";
        var body = BuildHostNotificationEmailBody(data);

        await SendEmailAsync(hostEmail, subject, body, data.IcsAttachment, cancellationToken);
    }

    private async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        string? icsAttachment,
        CancellationToken cancellationToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation(
                "Email sending is disabled. Would send to {Email}: {Subject}",
                toEmail, subject);
            return;
        }

        try
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = _settings.UseSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            if (!string.IsNullOrEmpty(icsAttachment))
            {
                var attachment = Attachment.CreateAttachmentFromString(
                    icsAttachment,
                    "invite.ics",
                    System.Text.Encoding.UTF8,
                    "text/calendar");
                message.Attachments.Add(attachment);
            }

            await client.SendMailAsync(message, cancellationToken);

            _logger.LogInformation(
                "Email sent successfully to {Email}: {Subject}",
                toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}: {Subject}", toEmail, subject);
            throw;
        }
    }

    private static string BuildConfirmationEmailBody(BookingEmailData data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #4f46e5; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9fafb; padding: 20px; border-radius: 0 0 8px 8px; }}
        .details {{ background: white; padding: 15px; border-radius: 8px; margin: 15px 0; }}
        .button {{ display: inline-block; background: #4f46e5; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; margin: 10px 5px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Booking Confirmed!</h1>
        </div>
        <div class='content'>
            <p>Hi {data.GuestName},</p>
            <p>Your booking with {data.HostName} has been confirmed.</p>

            <div class='details'>
                <h3>{data.EventTypeName}</h3>
                <p><strong>Date:</strong> {data.StartTimeUtc:dddd, MMMM d, yyyy}</p>
                <p><strong>Time:</strong> {data.StartTimeUtc:h:mm tt} - {data.EndTimeUtc:h:mm tt} (UTC)</p>
                {(string.IsNullOrEmpty(data.MeetingLink) ? "" : $"<p><strong>Meeting Link:</strong> <a href='{data.MeetingLink}'>{data.MeetingLink}</a></p>")}
                {(string.IsNullOrEmpty(data.LocationDetails) ? "" : $"<p><strong>Location:</strong> {data.LocationDetails}</p>")}
            </div>

            <p style='text-align: center;'>
                <a href='{data.RescheduleLink}' class='button'>Reschedule</a>
                <a href='{data.CancellationLink}' class='button' style='background: #dc2626;'>Cancel</a>
            </p>

            <div class='footer'>
                <p>A calendar invitation is attached to this email.</p>
                <p>Powered by ScheduleKit</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    private static string BuildCancellationEmailBody(BookingEmailData data, string? reason)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #dc2626; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9fafb; padding: 20px; border-radius: 0 0 8px 8px; }}
        .details {{ background: white; padding: 15px; border-radius: 8px; margin: 15px 0; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Booking Cancelled</h1>
        </div>
        <div class='content'>
            <p>Hi {data.GuestName},</p>
            <p>Your booking with {data.HostName} has been cancelled.</p>

            <div class='details'>
                <h3>{data.EventTypeName}</h3>
                <p><strong>Originally scheduled:</strong> {data.StartTimeUtc:dddd, MMMM d, yyyy} at {data.StartTimeUtc:h:mm tt} (UTC)</p>
                {(string.IsNullOrEmpty(reason) ? "" : $"<p><strong>Reason:</strong> {reason}</p>")}
            </div>

            <p>If you'd like to book a new time, please visit the booking page.</p>

            <div class='footer'>
                <p>Powered by ScheduleKit</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    private static string BuildRescheduledEmailBody(BookingEmailData data, DateTime oldStartTime)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #f59e0b; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9fafb; padding: 20px; border-radius: 0 0 8px 8px; }}
        .details {{ background: white; padding: 15px; border-radius: 8px; margin: 15px 0; }}
        .old-time {{ text-decoration: line-through; color: #999; }}
        .new-time {{ color: #059669; font-weight: bold; }}
        .button {{ display: inline-block; background: #4f46e5; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; margin: 10px 5px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Booking Rescheduled</h1>
        </div>
        <div class='content'>
            <p>Hi {data.GuestName},</p>
            <p>Your booking with {data.HostName} has been rescheduled.</p>

            <div class='details'>
                <h3>{data.EventTypeName}</h3>
                <p class='old-time'><strong>Old time:</strong> {oldStartTime:dddd, MMMM d, yyyy} at {oldStartTime:h:mm tt} (UTC)</p>
                <p class='new-time'><strong>New time:</strong> {data.StartTimeUtc:dddd, MMMM d, yyyy} at {data.StartTimeUtc:h:mm tt} - {data.EndTimeUtc:h:mm tt} (UTC)</p>
                {(string.IsNullOrEmpty(data.MeetingLink) ? "" : $"<p><strong>Meeting Link:</strong> <a href='{data.MeetingLink}'>{data.MeetingLink}</a></p>")}
            </div>

            <p style='text-align: center;'>
                <a href='{data.RescheduleLink}' class='button'>Reschedule Again</a>
                <a href='{data.CancellationLink}' class='button' style='background: #dc2626;'>Cancel</a>
            </p>

            <div class='footer'>
                <p>An updated calendar invitation is attached to this email.</p>
                <p>Powered by ScheduleKit</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    private static string BuildReminderEmailBody(BookingEmailData data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #4f46e5; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9fafb; padding: 20px; border-radius: 0 0 8px 8px; }}
        .details {{ background: white; padding: 15px; border-radius: 8px; margin: 15px 0; }}
        .button {{ display: inline-block; background: #4f46e5; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; margin: 10px 5px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Reminder: Upcoming Booking</h1>
        </div>
        <div class='content'>
            <p>Hi {data.GuestName},</p>
            <p>This is a reminder that you have an upcoming meeting with {data.HostName}.</p>

            <div class='details'>
                <h3>{data.EventTypeName}</h3>
                <p><strong>Date:</strong> {data.StartTimeUtc:dddd, MMMM d, yyyy}</p>
                <p><strong>Time:</strong> {data.StartTimeUtc:h:mm tt} - {data.EndTimeUtc:h:mm tt} (UTC)</p>
                {(string.IsNullOrEmpty(data.MeetingLink) ? "" : $"<p><strong>Meeting Link:</strong> <a href='{data.MeetingLink}'>{data.MeetingLink}</a></p>")}
            </div>

            <p style='text-align: center;'>
                <a href='{data.RescheduleLink}' class='button'>Reschedule</a>
                <a href='{data.CancellationLink}' class='button' style='background: #dc2626;'>Cancel</a>
            </p>

            <div class='footer'>
                <p>Powered by ScheduleKit</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    private static string BuildHostNotificationEmailBody(BookingEmailData data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #059669; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9fafb; padding: 20px; border-radius: 0 0 8px 8px; }}
        .details {{ background: white; padding: 15px; border-radius: 8px; margin: 15px 0; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>New Booking!</h1>
        </div>
        <div class='content'>
            <p>Hi {data.HostName},</p>
            <p>You have a new booking for {data.EventTypeName}.</p>

            <div class='details'>
                <h3>Guest: {data.GuestName}</h3>
                <p><strong>Email:</strong> <a href='mailto:{data.GuestEmail}'>{data.GuestEmail}</a></p>
                <p><strong>Date:</strong> {data.StartTimeUtc:dddd, MMMM d, yyyy}</p>
                <p><strong>Time:</strong> {data.StartTimeUtc:h:mm tt} - {data.EndTimeUtc:h:mm tt} (UTC)</p>
                <p><strong>Guest Timezone:</strong> {data.GuestTimezone}</p>
            </div>

            <div class='footer'>
                <p>A calendar invitation is attached to this email.</p>
                <p>Powered by ScheduleKit</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }
}

/// <summary>
/// Email configuration settings.
/// </summary>
public class EmailSettings
{
    public const string SectionName = "Email";

    public bool Enabled { get; set; }
    public string SmtpHost { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "noreply@schedulekit.com";
    public string FromName { get; set; } = "ScheduleKit";
}
