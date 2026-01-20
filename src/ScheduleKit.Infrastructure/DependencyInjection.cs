using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Infrastructure.Data;
using ScheduleKit.Infrastructure.Repositories;

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
        services.AddScoped<IEventTypeRepository, EventTypeRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<IAvailabilityOverrideRepository, AvailabilityOverrideRepository>();

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
