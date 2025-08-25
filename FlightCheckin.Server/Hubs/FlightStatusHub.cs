using FlightCheckin.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

using FlightCheckin.Models;
using Microsoft.AspNetCore.SignalR;

namespace FlightCheckin.Server.Hubs;

public class FlightStatusHub : Hub
{
    public Task JoinFlightGroup(string flightNumber) =>
        Groups.AddToGroupAsync(Context.ConnectionId, flightNumber);
}

