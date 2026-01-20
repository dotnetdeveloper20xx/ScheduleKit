using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Infrastructure.Data.Configurations;

public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
{
    public void Configure(EntityTypeBuilder<Availability> builder)
    {
        builder.ToTable("Availabilities");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.HostUserId)
            .IsRequired();

        builder.Property(a => a.DayOfWeek)
            .IsRequired();

        builder.Property(a => a.StartTime)
            .IsRequired();

        builder.Property(a => a.EndTime)
            .IsRequired();

        builder.Property(a => a.IsEnabled)
            .IsRequired();

        builder.Property(a => a.CreatedAtUtc)
            .IsRequired();

        builder.Property(a => a.UpdatedAtUtc);

        // Index for querying by host
        builder.HasIndex(a => a.HostUserId);

        // Unique constraint: one entry per day per host
        builder.HasIndex(a => new { a.HostUserId, a.DayOfWeek })
            .IsUnique();

        builder.Ignore(a => a.DomainEvents);
    }
}

public class AvailabilityOverrideConfiguration : IEntityTypeConfiguration<AvailabilityOverride>
{
    public void Configure(EntityTypeBuilder<AvailabilityOverride> builder)
    {
        builder.ToTable("AvailabilityOverrides");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.HostUserId)
            .IsRequired();

        builder.Property(a => a.Date)
            .IsRequired();

        builder.Property(a => a.StartTime);

        builder.Property(a => a.EndTime);

        builder.Property(a => a.IsBlocked)
            .IsRequired();

        builder.Property(a => a.Reason)
            .HasMaxLength(200);

        builder.Property(a => a.CreatedAtUtc)
            .IsRequired();

        builder.Property(a => a.UpdatedAtUtc);

        // Index for querying by host and date range
        builder.HasIndex(a => a.HostUserId);
        builder.HasIndex(a => new { a.HostUserId, a.Date });

        builder.Ignore(a => a.DomainEvents);
    }
}
