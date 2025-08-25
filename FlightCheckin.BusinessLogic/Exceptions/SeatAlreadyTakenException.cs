using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace FlightCheckin.BusinessLogic.Exceptions;

[Serializable]
public class SeatAlreadyTakenException : Exception
{
    public SeatAlreadyTakenException(string seat) : base($"Seat {seat} already taken.") { }
    protected SeatAlreadyTakenException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

