using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Infrastructure.Data.Configurations;

public class BookingQuestionConfiguration : IEntityTypeConfiguration<BookingQuestion>
{
    public void Configure(EntityTypeBuilder<BookingQuestion> builder)
    {
        builder.ToTable("BookingQuestions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.EventTypeId)
            .IsRequired();

        builder.Property(q => q.QuestionText)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(q => q.Type)
            .IsRequired();

        builder.Property(q => q.IsRequired)
            .IsRequired();

        // Store options as JSON
        builder.Property(q => q.Options)
            .HasConversion(
                options => JsonSerializer.Serialize(options, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<List<string>>(json, JsonSerializerOptions.Default) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.DisplayOrder)
            .IsRequired();

        builder.Property(q => q.CreatedAtUtc)
            .IsRequired();

        builder.Property(q => q.UpdatedAtUtc);

        builder.HasIndex(q => q.EventTypeId);

        builder.Ignore(q => q.DomainEvents);
    }
}
