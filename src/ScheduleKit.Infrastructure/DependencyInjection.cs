using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Domain.Services;
using ScheduleKit.Infrastructure.Data;
using ScheduleKit.Infrastructure.Repositories;
using ScheduleKit.Infrastructure.Services;

namespace ScheduleKit.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure services including EF Core and repositories.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Action<ScheduleKitDbOptions> configureOptions)
    {
        var options = new ScheduleKitDbOptions();
        configureOptions(options);

        // Configure DbContext based on options
        services.AddDbContext<ScheduleKitDbContext>(dbOptions =>
        {
            if (options.UseInMemory)
            {
                dbOptions.UseInMemoryDatabase(options.DatabaseName ?? "ScheduleKit");
            }
            else if (!string.IsNullOrEmpty(options.ConnectionString))
            {
                dbOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(3);
                    sqlOptions.CommandTimeout(30);
                });
            }
            else
            {
                throw new InvalidOperationException(
                    "Either UseInMemory must be true or ConnectionString must be provided.");
            }
        });

        // Register Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ScheduleKitDbContext>());

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventTypeRepository, EventTypeRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<IAvailabilityOverrideRepository, AvailabilityOverrideRepository>();

        // Register services
        services.AddScoped<ISlotCalculator, SlotCalculator>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    /// <summary>
    /// Adds external integration services (calendar, video conferencing, OAuth).
    /// Uses mock implementations by default, or real implementations when configured.
    /// </summary>
    public static IServiceCollection AddExternalIntegrations(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = new ExternalIntegrationSettings();
        var section = configuration.GetSection(ExternalIntegrationSettings.SectionName);
        section.Bind(settings);

        services.Configure<ExternalIntegrationSettings>(opts =>
        {
            opts.UseMockServices = settings.UseMockServices;
            opts.Calendar = settings.Calendar;
            opts.VideoConference = settings.VideoConference;
            opts.OAuth = settings.OAuth;
        });

        if (settings.UseMockServices)
        {
            // Register mock implementations for demo/development
            services.AddScoped<IExternalCalendarService, MockCalendarService>();
            services.AddScoped<IVideoConferenceService, MockVideoConferenceService>();
            services.AddSingleton<IOAuthService, MockOAuthService>();
        }
        else
        {
            // TODO: Register real implementations when API keys are configured
            // For now, fall back to mock if real implementations aren't available

            // Calendar service - check if Google or Microsoft credentials are configured
            if (!string.IsNullOrEmpty(settings.Calendar?.Google?.ClientId) ||
                !string.IsNullOrEmpty(settings.Calendar?.Microsoft?.ClientId))
            {
                // TODO: services.AddScoped<IExternalCalendarService, GoogleCalendarService>();
                // For now, use mock
                services.AddScoped<IExternalCalendarService, MockCalendarService>();
            }
            else
            {
                services.AddScoped<IExternalCalendarService, MockCalendarService>();
            }

            // Video conference service - check if Zoom or other credentials are configured
            if (!string.IsNullOrEmpty(settings.VideoConference?.Zoom?.ClientId))
            {
                // TODO: services.AddScoped<IVideoConferenceService, ZoomService>();
                // For now, use mock
                services.AddScoped<IVideoConferenceService, MockVideoConferenceService>();
            }
            else
            {
                services.AddScoped<IVideoConferenceService, MockVideoConferenceService>();
            }

            // OAuth service - check if any OAuth providers are configured
            if (!string.IsNullOrEmpty(settings.OAuth?.Google?.ClientId) ||
                !string.IsNullOrEmpty(settings.OAuth?.Microsoft?.ClientId) ||
                !string.IsNullOrEmpty(settings.OAuth?.GitHub?.ClientId))
            {
                // TODO: services.AddSingleton<IOAuthService, RealOAuthService>();
                // For now, use mock
                services.AddSingleton<IOAuthService, MockOAuthService>();
            }
            else
            {
                services.AddSingleton<IOAuthService, MockOAuthService>();
            }
        }

        return services;
    }

    /// <summary>
    /// Adds email service configuration from configuration section.
    /// </summary>
    public static IServiceCollection AddEmailConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSettings = new EmailSettings();
        configuration.GetSection(EmailSettings.SectionName).Bind(emailSettings);
        services.Configure<EmailSettings>(opts =>
        {
            opts.Enabled = emailSettings.Enabled;
            opts.SmtpHost = emailSettings.SmtpHost;
            opts.SmtpPort = emailSettings.SmtpPort;
            opts.UseSsl = emailSettings.UseSsl;
            opts.Username = emailSettings.Username;
            opts.Password = emailSettings.Password;
            opts.FromEmail = emailSettings.FromEmail;
            opts.FromName = emailSettings.FromName;
        });

        return services;
    }

    /// <summary>
    /// Adds JWT authentication configuration from configuration section.
    /// </summary>
    public static IServiceCollection AddJwtConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);

        // Use a default secret for development if not configured
        if (string.IsNullOrEmpty(jwtSettings.Secret))
        {
            jwtSettings.Secret = "ScheduleKit-Development-Secret-Key-Min-32-Chars!";
        }

        services.Configure<JwtSettings>(opts =>
        {
            opts.Secret = jwtSettings.Secret;
            opts.Issuer = jwtSettings.Issuer;
            opts.Audience = jwtSettings.Audience;
            opts.ExpirationMinutes = jwtSettings.ExpirationMinutes;
            opts.RefreshTokenExpirationDays = jwtSettings.RefreshTokenExpirationDays;
        });

        return services;
    }
}

/// <summary>
/// Configuration options for ScheduleKit database.
/// </summary>
public class ScheduleKitDbOptions
{
    /// <summary>
    /// Use in-memory database (for development/testing).
    /// </summary>
    public bool UseInMemory { get; set; }

    /// <summary>
    /// Database name for in-memory database.
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// Connection string for SQL Server.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Configure to use in-memory database.
    /// </summary>
    public void UseInMemoryDatabase(string databaseName = "ScheduleKit")
    {
        UseInMemory = true;
        DatabaseName = databaseName;
    }

    /// <summary>
    /// Configure to use SQL Server.
    /// </summary>
    public void UseSqlServer(string connectionString)
    {
        UseInMemory = false;
        ConnectionString = connectionString;
    }
}
