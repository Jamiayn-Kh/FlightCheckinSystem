using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightCheckin.Models;

namespace FlightCheckin.BusinessLogic.Validators;

public static class CheckinValidator
{
    public static void Validate(CheckinRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.FlightNumber))
            throw new ArgumentException("FlightNumber is required.");
        if (string.IsNullOrWhiteSpace(req.PassportNumber))
            throw new ArgumentException("PassportNumber is required.");
        if ((req.SeatRow.HasValue && string.IsNullOrWhiteSpace(req.SeatColumn)) ||
            (!req.SeatRow.HasValue && !string.IsNullOrWhiteSpace(req.SeatColumn)))
            throw new ArgumentException("Both SeatRow and SeatColumn required together, or both null.");
    }
}

