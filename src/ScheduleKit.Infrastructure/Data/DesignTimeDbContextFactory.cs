using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ScheduleKit.Infrastructure.Data;

/// <summary>
/// Factory for creating DbContext at design time for EF Core migrations.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ScheduleKitDbContext>
{
    public ScheduleKitDbContext CreateDbContext(string[] args)
    {
        // Use a default LocalDB connection for design-time migrations
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ScheduleKit;Trusted_Connection=True;MultipleActiveResultSets=true";

        var optionsBuilder = new DbContextOptionsBuilder<ScheduleKitDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ScheduleKitDbContext(optionsBuilder.Options);
    }
}
