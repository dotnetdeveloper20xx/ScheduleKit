using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleKit.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingAndCalendarFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalendarEventId",
                table: "Bookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendarLink",
                table: "Bookings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalMeetingId",
                table: "Bookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetingPassword",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalendarEventId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CalendarLink",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ExternalMeetingId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "MeetingPassword",
                table: "Bookings");
        }
    }
}
