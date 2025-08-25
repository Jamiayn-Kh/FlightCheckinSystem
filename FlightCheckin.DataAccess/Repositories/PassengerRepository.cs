using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightCheckin.DataAccess.Context;
using FlightCheckin.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightCheckin.DataAccess.Repositories;

public class PassengerRepository : IPassengerRepository
{
    private readonly FlightContext _db;
    public PassengerRepository(FlightContext db) => _db = db;

    public Task<Passenger?> FindByPassportAsync(string passportNumber, CancellationToken ct) =>
        _db.Passengers.FirstOrDefaultAsync(p => p.PassportNumber == passportNumber, ct);

    public async Task<Passenger> GetOrCreateAsync(string passportNumber, string fullName, CancellationToken ct)
    {
        var p = await FindByPassportAsync(passportNumber, ct);
        if (p is not null) return p;
        p = new Passenger { PassportNumber = passportNumber, FullName = fullName };
        _db.Passengers.Add(p);
        await _db.SaveChangesAsync(ct);
        return p;
    }
}

