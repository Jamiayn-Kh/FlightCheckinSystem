using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models;

public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = default!;
    public string Destination { get; set; } = default!;
    public DateTime DepartureTime { get; set; }
    public FlightStatus Status { get; set; } = FlightStatus.CheckingIn;

    public List<Seat> Seats { get; set; } = new();
    public List<Passenger> Passengers { get; set; } = new();
}

