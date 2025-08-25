using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCheckin.Models
{
    public enum FlightStatus
    {
        CheckingIn,      // Бүртгэж байна
        Boarding,        // Онгоцонд сууж байна
        Departed,        // Ниссэн
        Delayed,         // Хойшилсон
        Cancelled        // Цуцалсан
    }
}
