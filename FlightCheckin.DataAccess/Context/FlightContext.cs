using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightCheckin.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightCheckin.DataAccess.Context;

public class FlightContext : DbContext
{
    public FlightContext(DbContextOptions<FlightContext> options) : base(options) { }

    // Design-time factory for migrations
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "flights.db");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }
    }

    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<BoardingPass> BoardingPasses => Set<BoardingPass>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Passenger>()
            .HasIndex(p => p.PassportNumber)
            .IsUnique();

        modelBuilder.Entity<Seat>()
            .HasIndex(s => new { s.FlightId, s.Row, s.Column })
            .IsUnique();

        modelBuilder.Entity<Seat>()
            .HasOne(s => s.AssignedPassenger)
            .WithMany()
            .HasForeignKey(s => s.AssignedPassengerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed бага өгөгдөл
        var seedFlightId = 1;
        modelBuilder.Entity<Flight>().HasData(new Flight
        {
            Id = seedFlightId,
            FlightNumber = "MGL101",
            Destination = "Tokyo (NRT)",
            DepartureTime = DateTime.UtcNow.AddHours(6),
            Status = FlightStatus.CheckingIn
        });

        // 6 мөр, A–F  (жишээ суудал)
        var seats = new List<Seat>();
        string[] cols = ["A", "B", "C", "D", "E", "F"];
        int seatId = 1;
        for (int r = 1; r <= 10; r++)
            foreach (var c in cols)
                seats.Add(new Seat { Id = seatId++, FlightId = seedFlightId, Row = r, Column = c, IsTaken = false });
        modelBuilder.Entity<Seat>().HasData(seats);
    }
}

