using FlightCheckin.Models;
using Microsoft.AspNetCore.SignalR;

namespace FlightCheckin.Server.Hubs;

public class FlightStatusHub : Hub
{
    public Task JoinFlightGroup(string flightNumber) =>
        Groups.AddToGroupAsync(Context.ConnectionId, flightNumber);

    public Task UpdateFlightStatus(string flightNumber, FlightStatus status) =>
        Clients.Group(flightNumber).SendAsync("FlightStatusUpdated", flightNumber, status);

    public Task NotifySeatAssigned(string flightNumber, string seatCode) =>
        Clients.Group(flightNumber).SendAsync("SeatAssigned", flightNumber, seatCode);
}

