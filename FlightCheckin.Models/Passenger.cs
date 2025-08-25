using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models;

public class Passenger
{
    public int Id {get; set;}
    public string PassportNumber {get; set; } = default!; // unique
    public string FullName { get; set; } = default!;
}

