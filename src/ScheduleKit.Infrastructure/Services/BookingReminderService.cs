using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Infrastructure.Services;

/// <summary>
/// Background service that sends booking reminders.
/// </summary>
public class BookingReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BookingReminderService> _logger;
    private readonly ReminderSettings _settings;

    public BookingReminderService(
        IServiceProvider serviceProvider,
        ILogger<BookingReminderService> logger,
        IOptions<ReminderSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("Booking reminder service is disabled");
            return;
        }

        _logger.LogInformation(
            "Booking reminder service started. Checking every {Interval} minutes, sending reminders {Hours}h before bookings",
            _settings.CheckIntervalMinutes,
            _settings.ReminderHoursBefore);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing booking reminders");
            }

            await Task.Delay(TimeSpan.FromMinutes(_settings.CheckIntervalMinutes), stoppingToken);
        }
    }

    private async Task ProcessRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var bookings = await bookingRepository.GetBookingsNeedingReminderAsync(
            _settings.ReminderHoursBefore,
            _settings.BatchSize,
            cancellationToken);

        if (bookings.Count == 0)
        {
            _logger.LogDebug("No bookings need reminders at this time");
            return;
        }

        _logger.LogInformation("Found {Count} bookings needing reminders", bookings.Count);

        foreach (var booking in bookings)
        {
            try
            {
                // Get host info for the email
                var host = await userRepository.GetByIdAsync(booking.HostUserId, cancellationToken);
                var hostName = host?.Name ?? "Host";

                var emailData = new BookingEmailData
                {
                    BookingId = booking.Id,
                    EventTypeName = booking.EventType?.Name ?? "Meeting",
                    HostName = hostName,
                    GuestName = booking.GuestName,
                    GuestEmail = booking.GuestEmail,
                    StartTimeUtc = booking.StartTimeUtc,
                    EndTimeUtc = booking.EndTimeUtc,
                    GuestTimezone = booking.GuestTimezone,
                    MeetingLink = booking.MeetingLink,
                    CancellationLink = $"https://app.schedulekit.com/cancel/{booking.Id}",
                    RescheduleLink = $"https://app.schedulekit.com/reschedule/{booking.RescheduleToken}"
                };

                await emailService.SendBookingReminderAsync(emailData, cancellationToken);

                // Mark reminder as sent
                booking.MarkReminderSent();

                _logger.LogInformation(
                    "Sent reminder for booking {BookingId} to {GuestEmail}",
                    booking.Id,
                    booking.GuestEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send reminder for booking {BookingId}",
                    booking.Id);
            }
        }

        // Save changes for all processed bookings
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Configuration settings for booking reminders.
/// </summary>
public class ReminderSettings
{
    public const string SectionName = "Reminders";

    /// <summary>
    /// Whether the reminder service is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Hours before the booking to send reminder.
    /// </summary>
    public int ReminderHoursBefore { get; set; } = 24;

    /// <summary>
    /// How often to check for reminders (in minutes).
    /// </summary>
    public int CheckIntervalMinutes { get; set; } = 15;

    /// <summary>
    /// Maximum number of reminders to process in one batch.
    /// </summary>
    public int BatchSize { get; set; } = 100;
}
