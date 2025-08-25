using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models;

public class Seat
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public Flight? Flight { get; set; }

    public int Row { get; set; }
    public string Column { get; set; } = "A";
    public bool IsTaken { get; set; }

    public int? AssignedPassengerId { get; set; }
    public Passenger? AssignedPassenger { get; set; }

    public override string ToString() => $"{Row}{Column}";
}

