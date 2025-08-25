using FlightCheckin.BusinessLogic.Services;
using FlightCheckin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using FlightCheckin.Server.Hubs;

namespace FlightCheckin.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckinController : ControllerBase
{
    private readonly ICheckinService _checkin;
    private readonly IHubContext<FlightStatusHub> _hub;

    public CheckinController(ICheckinService checkin, IHubContext<FlightStatusHub> hub)
    {
        _checkin = checkin;
        _hub = hub;
    }

    [HttpGet("seats/{flightNumber}")]
    public Task<List<SeatDto>> Seats(string flightNumber, CancellationToken ct) =>
        _checkin.GetSeatsAsync(flightNumber, ct);

    [HttpPost]
    public async Task<ActionResult<CheckinResponse>> Post([FromBody] CheckinRequest req, CancellationToken ct)
    {
        var res = await _checkin.AssignSeatAsync(req, ct);
        
        // Send real-time notifications to all clients in the flight group
        await _hub.Clients.Group(req.FlightNumber).SendAsync("SeatAssigned", req.FlightNumber, res.SeatCode, ct);
        
        // Also send to all clients to refresh seat data
        await _hub.Clients.All.SendAsync("SeatsUpdated", req.FlightNumber, ct);
        
        return res;
    }
}
