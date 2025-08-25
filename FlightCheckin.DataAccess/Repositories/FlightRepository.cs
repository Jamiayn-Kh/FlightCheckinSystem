using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightCheckin.DataAccess.Context;
using FlightCheckin.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightCheckin.DataAccess.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly FlightContext _db;
    public FlightRepository(FlightContext db) => _db = db;

    public Task<Flight?> GetByNumberAsync(string flightNumber, CancellationToken ct) =>
        _db.Flights
           .Include(f => f.Seats)
           .Include(f => f.Passengers)
           .FirstOrDefaultAsync(f => f.FlightNumber == flightNumber, ct);

    public async Task UpdateAsync(Flight flight, CancellationToken ct)
    {
        _db.Flights.Update(flight);
        await _db.SaveChangesAsync(ct);
    }

    public Task<List<Flight>> GetAllAsync(CancellationToken ct) =>
        _db.Flights.AsNoTracking().ToListAsync(ct);
}

