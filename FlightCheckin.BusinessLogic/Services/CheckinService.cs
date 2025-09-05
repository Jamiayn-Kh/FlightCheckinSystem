using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightCheckin.BusinessLogic.Exceptions;
using FlightCheckin.BusinessLogic.Validators;
using FlightCheckin.DataAccess.Context;
using FlightCheckin.DataAccess.Repositories;
using FlightCheckin.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightCheckin.BusinessLogic.Services;

public class CheckinService : ICheckinService
{
    private readonly FlightContext _db;
    private readonly IFlightRepository _flights;
    private readonly IPassengerRepository _passengers;

    public CheckinService(FlightContext db, IFlightRepository flights, IPassengerRepository passengers)
    {
        _db = db;
        _flights = flights;
        _passengers = passengers;
    }

    public async Task<CheckinResponse> AssignSeatAsync(CheckinRequest request, CancellationToken ct)
    {
        CheckinValidator.Validate(request);

        // зорчигчийн нэрийг desktop-аас авч чадаагүй тохиолдолд паспортын дугаарыг нэр болгон ашиглая
        var providedName = string.IsNullOrWhiteSpace(request.PassengerName)
            ? request.PassportNumber
            : request.PassengerName.Trim();
        var passenger = await _passengers.GetOrCreateAsync(request.PassportNumber.Trim(), providedName, ct);
        var flight = await _flights.GetByNumberAsync(request.FlightNumber, ct)
                     ?? throw new InvalidOperationException("Flight not found");

        // Пессимист түгжих — нэг суудлыг зэрэг эзлэх оролдлогыг хаах
        using var tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);

        Seat seat;
        if (request.SeatRow is not null)
        {
            seat = await _db.Seats
                .Where(s => s.FlightId == flight.Id && s.Row == request.SeatRow && s.Column == request.SeatColumn!)
                .FirstOrDefaultAsync(ct) ?? throw new InvalidOperationException("Seat not found");
            await _db.Entry(seat).ReloadAsync(ct); // шинэчил
            if (seat.IsTaken) throw new SeatAlreadyTakenException($"{seat.Row}{seat.Column}");
        }
        else
        {
            seat = await _db.Seats
                .Where(s => s.FlightId == flight.Id && !s.IsTaken)
                .OrderBy(s => s.Row).ThenBy(s => s.Column)
                .FirstOrDefaultAsync(ct) ?? throw new InvalidOperationException("No seats left");
        }

        seat.IsTaken = true;
        seat.AssignedPassengerId = passenger.Id;
        await _db.SaveChangesAsync(ct);

        var pass = new BoardingPass
        {
            FlightId = flight.Id,
            FlightNumber = flight.FlightNumber,
            PassengerId = passenger.Id,
            PassengerName = passenger.FullName,
            PassportNumber = passenger.PassportNumber,
            SeatCode = $"{seat.Row}{seat.Column}",
            IssuedAt = DateTime.UtcNow
        };
        _db.BoardingPasses.Add(pass);
        await _db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);

        return new CheckinResponse(true, "Seat assigned", pass.SeatCode, pass);
    }

    public async Task<List<SeatDto>> GetSeatsAsync(string flightNumber, CancellationToken ct)
    {
        var flight = await _flights.GetByNumberAsync(flightNumber, ct)
                     ?? throw new InvalidOperationException("Flight not found");
        var result = await _db.Seats
            .Where(s => s.FlightId == flight.Id)
            .OrderBy(s => s.Row).ThenBy(s => s.Column)
            .Select(s => new SeatDto(
                s.Id, s.Row, s.Column, s.IsTaken,
                s.AssignedPassenger != null ? s.AssignedPassenger.FullName : null))
            .ToListAsync(ct);
        return result;
    }
}

