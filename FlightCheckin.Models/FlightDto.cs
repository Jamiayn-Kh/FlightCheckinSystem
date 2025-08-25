using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models
{
    public class FlightDto
    {
        public string FlightNumber { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public DateTime DepartureTime { get; set; }
        public FlightStatus Status { get; set; }

        public FlightDto(string flightNumber, string destination, DateTime departureTime, FlightStatus status)
        {
            FlightNumber = flightNumber;
            Destination = destination;
            DepartureTime = departureTime;
            Status = status;
        }
    }
}



