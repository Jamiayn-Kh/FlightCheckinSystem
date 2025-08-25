using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightCheckin.Models;

namespace FlightCheckin.BusinessLogic.Services;

public interface ICheckinService
{
    Task<CheckinResponse> AssignSeatAsync(CheckinRequest request, CancellationToken ct);
    Task<List<SeatDto>> GetSeatsAsync(string flightNumber, CancellationToken ct);
}

