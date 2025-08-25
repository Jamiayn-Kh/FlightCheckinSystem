using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models;

public class BoardingPass
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public string FlightNumber { get; set; } = default!;
    public int PassengerId { get; set; }
    public string PassengerName { get; set; } = default!;
    public string PassportNumber { get; set; } = default!;
    public string SeatCode { get; set; } = default!;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
}

