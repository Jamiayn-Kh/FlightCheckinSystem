using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models
{
    public class CheckinResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public string SeatCode { get; set; } = default!;
        public BoardingPass? BoardingPass { get; set; }

        public CheckinResponse(bool success, string message, string seatCode, BoardingPass? boardingPass = null)
        {
            Success = success;
            Message = message;
            SeatCode = seatCode;
            BoardingPass = boardingPass;
        }
    }
}



