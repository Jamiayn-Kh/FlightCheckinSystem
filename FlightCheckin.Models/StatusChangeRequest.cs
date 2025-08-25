using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models
{
    public class StatusChangeRequest
    {
        public string FlightNumber { get; set; } = default!;
        public FlightStatus Status { get; set; }
    }
}



