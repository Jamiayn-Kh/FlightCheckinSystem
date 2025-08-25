using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightCheckin.Models;

namespace FlightCheckin.BusinessLogic.Services;

public interface IFlightService
{
    Task<List<FlightDto>> GetFlightsAsync(CancellationToken ct);
    Task<FlightDto?> GetFlightAsync(string flightNumber, CancellationToken ct);
    Task<FlightDto> ChangeStatusAsync(string flightNumber, FlightStatus status, CancellationToken ct);
}

