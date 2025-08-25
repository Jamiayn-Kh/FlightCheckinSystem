using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models
{
    public class CheckinRequest
    {
        public string FlightNumber { get; set; } = default!;
        public string PassportNumber { get; set; } = default!;
        public string? PassengerName { get; set; }
        public int? SeatRow { get; set; }
        public string? SeatColumn { get; set; }
    }
}
