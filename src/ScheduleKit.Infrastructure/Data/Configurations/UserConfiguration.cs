using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleKit.Domain.Entities;

namespace ScheduleKit.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Slug)
            .HasMaxLength(50);

        builder.HasIndex(u => u.Slug)
            .IsUnique()
            .HasFilter("[Slug] IS NOT NULL");

        builder.Property(u => u.Timezone)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("UTC");

        builder.Property(u => u.EmailNotificationsEnabled)
            .HasDefaultValue(true);

        builder.Property(u => u.ReminderEmailsEnabled)
            .HasDefaultValue(true);

        builder.Property(u => u.ReminderHoursBefore)
            .HasDefaultValue(24);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAtUtc)
            .IsRequired();

        builder.Ignore(u => u.DomainEvents);
    }
}
