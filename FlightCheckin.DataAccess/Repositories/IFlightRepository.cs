using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightCheckin.Models;

namespace FlightCheckin.DataAccess.Repositories;

public interface IFlightRepository
{
    Task<Flight?> GetByNumberAsync(string flightNumber, CancellationToken ct);
    Task UpdateAsync(Flight flight, CancellationToken ct);
    Task<List<Flight>> GetAllAsync(CancellationToken ct);
}

