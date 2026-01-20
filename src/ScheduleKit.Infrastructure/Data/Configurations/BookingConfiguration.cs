using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.EventTypeId)
            .IsRequired();

        builder.Property(b => b.HostUserId)
            .IsRequired();

        builder.Property(b => b.GuestName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.GuestEmail)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(b => b.GuestPhone)
            .HasMaxLength(50);

        builder.Property(b => b.GuestNotes)
            .HasMaxLength(2000);

        builder.Property(b => b.StartTimeUtc)
            .IsRequired();

        builder.Property(b => b.EndTimeUtc)
            .IsRequired();

        builder.Property(b => b.GuestTimezone)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Status)
            .IsRequired();

        builder.Property(b => b.CancellationReason)
            .HasMaxLength(500);

        builder.Property(b => b.CancelledAtUtc);

        builder.Property(b => b.RescheduleToken)
            .HasMaxLength(100);

        builder.Property(b => b.MeetingLink)
            .HasMaxLength(500);

        builder.Property(b => b.CreatedAtUtc)
            .IsRequired();

        builder.Property(b => b.UpdatedAtUtc);

        // Indexes for common queries
        builder.HasIndex(b => b.HostUserId);
        builder.HasIndex(b => b.EventTypeId);
        builder.HasIndex(b => b.GuestEmail);
        builder.HasIndex(b => b.RescheduleToken);
        builder.HasIndex(b => new { b.HostUserId, b.StartTimeUtc });
        builder.HasIndex(b => new { b.HostUserId, b.Status });

        // Relationship with EventType
        builder.HasOne(b => b.EventType)
            .WithMany()
            .HasForeignKey(b => b.EventTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship with BookingQuestionResponses
        builder.HasMany(b => b.Responses)
            .WithOne()
            .HasForeignKey(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(b => b.DomainEvents);
    }
}

public class BookingQuestionResponseConfiguration : IEntityTypeConfiguration<BookingQuestionResponse>
{
    public void Configure(EntityTypeBuilder<BookingQuestionResponse> builder)
    {
        builder.ToTable("BookingQuestionResponses");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.BookingId)
            .IsRequired();

        builder.Property(r => r.QuestionId)
            .IsRequired();

        builder.Property(r => r.ResponseValue)
            .HasMaxLength(5000);

        builder.Property(r => r.CreatedAtUtc)
            .IsRequired();

        builder.Property(r => r.UpdatedAtUtc);

        builder.HasIndex(r => r.BookingId);

        builder.Ignore(r => r.DomainEvents);
    }
}
