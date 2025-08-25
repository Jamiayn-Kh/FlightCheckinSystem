using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightCheckin.Models;

namespace FlightCheckin.DataAccess.Repositories;

public interface IPassengerRepository
{
    Task<Passenger?> FindByPassportAsync(string passportNumber, CancellationToken ct);
    Task<Passenger> GetOrCreateAsync(string passportNumber, string fullName, CancellationToken ct);
}

