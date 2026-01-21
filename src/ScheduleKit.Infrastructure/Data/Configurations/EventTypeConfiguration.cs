using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Infrastructure.Data.Configurations;

public class EventTypeConfiguration : IEntityTypeConfiguration<EventType>
{
    public void Configure(EntityTypeBuilder<EventType> builder)
    {
        builder.ToTable("EventTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.HostUserId)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Configure Slug value object
        builder.Property(e => e.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.FromString(value))
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        // Configure Duration value object
        builder.Property(e => e.Duration)
            .HasConversion(
                d => d.Minutes,
                v => Duration.FromMinutes(v))
            .IsRequired();

        // Configure BufferTime value objects
        builder.Property(e => e.BufferBefore)
            .HasConversion(
                b => b.Minutes,
                v => BufferTime.FromMinutes(v))
            .IsRequired();

        builder.Property(e => e.BufferAfter)
            .HasConversion(
                b => b.Minutes,
                v => BufferTime.FromMinutes(v))
            .IsRequired();

        // Configure MinimumNotice value object
        builder.Property(e => e.MinimumNotice)
            .HasConversion(
                m => m.Minutes,
                v => MinimumNotice.FromMinutes(v))
            .IsRequired()
            .HasDefaultValue(MinimumNotice.OneHour);

        // Configure BookingWindow value object
        builder.Property(e => e.BookingWindow)
            .HasConversion(
                b => b.Days,
                v => BookingWindow.FromDays(v))
            .IsRequired()
            .HasDefaultValue(BookingWindow.SixtyDays);

        // Configure MaxBookingsPerDay (nullable)
        builder.Property(e => e.MaxBookingsPerDay)
            .IsRequired(false);

        // Configure MeetingLocation as owned entity
        builder.OwnsOne(e => e.Location, location =>
        {
            location.Property(l => l.Type)
                .HasColumnName("LocationType")
                .IsRequired();

            location.Property(l => l.Details)
                .HasColumnName("LocationDetails")
                .HasMaxLength(500);

            location.Property(l => l.DisplayName)
                .HasColumnName("LocationDisplayName")
                .HasMaxLength(100);
        });

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.Color)
            .HasMaxLength(7); // #RRGGBB

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();

        builder.Property(e => e.UpdatedAtUtc);

        // Index for querying by host
        builder.HasIndex(e => e.HostUserId);

        // Unique index for slug per host
        builder.HasIndex(e => new { e.HostUserId, e.Slug })
            .IsUnique();

        // Configure relationship with BookingQuestions
        builder.HasMany(e => e.Questions)
            .WithOne()
            .HasForeignKey(q => q.EventTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);
    }
}
