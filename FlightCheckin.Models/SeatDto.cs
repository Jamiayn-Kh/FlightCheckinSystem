using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models
{
    public class SeatDto
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public string Column { get; set; } = default!;
        public bool IsTaken { get; set; }
        public string? AssignedPassengerName { get; set; }

        public SeatDto(int id, int row, string column, bool isTaken, string? assignedPassengerName)
        {
            Id = id;
            Row = row;
            Column = column;
            IsTaken = isTaken;
            AssignedPassengerName = assignedPassengerName;
        }
    }
}



