using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlightCheckin.DataAccess.Context;

public class FlightContextFactory : IDesignTimeDbContextFactory<FlightContext>
{
    public FlightContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FlightContext>();
        var path = Path.Combine(AppContext.BaseDirectory, "flights.db");
        optionsBuilder.UseSqlite($"Data Source={path}");

        return new FlightContext(optionsBuilder.Options);
    }
}



