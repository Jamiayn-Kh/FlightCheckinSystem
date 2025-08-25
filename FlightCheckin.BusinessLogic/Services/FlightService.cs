using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightCheckin.DataAccess.Repositories;
using FlightCheckin.Models;

namespace FlightCheckin.BusinessLogic.Services;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _repo;

    public FlightService(IFlightRepository repo) => _repo = repo;

    public async Task<List<FlightDto>> GetFlightsAsync(CancellationToken ct)
        => (await _repo.GetAllAsync(ct))
           .Select(f => new FlightDto(f.FlightNumber, f.Destination, f.DepartureTime, f.Status))
           .ToList();

    public async Task<FlightDto?> GetFlightAsync(string flightNumber, CancellationToken ct)
    {
        var f = await _repo.GetByNumberAsync(flightNumber, ct);
        return f is null ? null : new FlightDto(f.FlightNumber, f.Destination, f.DepartureTime, f.Status);
    }

    public async Task<FlightDto> ChangeStatusAsync(string flightNumber, FlightStatus status, CancellationToken ct)
    {
        var f = await _repo.GetByNumberAsync(flightNumber, ct) ?? throw new InvalidOperationException("Flight not found");
        f.Status = status;
        await _repo.UpdateAsync(f, ct);
        return new FlightDto(f.FlightNumber, f.Destination, f.DepartureTime, f.Status);
    }
}

