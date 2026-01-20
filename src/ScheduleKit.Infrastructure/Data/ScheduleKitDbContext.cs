using Microsoft.EntityFrameworkCore;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;

namespace ScheduleKit.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for ScheduleKit.
/// </summary>
public class ScheduleKitDbContext : DbContext, IUnitOfWork
{
    public DbSet<EventType> EventTypes => Set<EventType>();
    public DbSet<BookingQuestion> BookingQuestions => Set<BookingQuestion>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<AvailabilityOverride> AvailabilityOverrides => Set<AvailabilityOverride>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingQuestionResponse> BookingQuestionResponses => Set<BookingQuestionResponse>();

    public ScheduleKitDbContext(DbContextOptions<ScheduleKitDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScheduleKitDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        var domainEvents = GetDomainEvents();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Domain events would be dispatched here via MediatR
        // For now, we'll clear them after save
        ClearDomainEvents();

        return result;
    }

    private List<IDomainEvent> GetDomainEvents()
    {
        return ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
    }

    private void ClearDomainEvents()
    {
        ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList()
            .ForEach(e => e.Entity.ClearDomainEvents());
    }
}
