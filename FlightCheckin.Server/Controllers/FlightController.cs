using FlightCheckin.BusinessLogic.Services;
using FlightCheckin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using FlightCheckin.Server.Hubs;

namespace FlightCheckin.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightController : ControllerBase
{
    private readonly IFlightService _service;
    private readonly IHubContext<FlightStatusHub> _hub;

    public FlightController(IFlightService service, IHubContext<FlightStatusHub> hub)
    {
        _service = service;
        _hub = hub;
    }

    [HttpGet]
    public async Task<ActionResult<List<FlightDto>>> GetAll(CancellationToken ct) =>
        await _service.GetFlightsAsync(ct);

    [HttpGet("{flightNumber}")]
    public async Task<ActionResult<FlightDto?>> Get(string flightNumber, CancellationToken ct) =>
        await _service.GetFlightAsync(flightNumber, ct);

    [HttpPut("status")]
    public async Task<ActionResult<FlightDto>> ChangeStatus([FromBody] StatusChangeRequest req, CancellationToken ct)
    {
        var dto = await _service.ChangeStatusAsync(req.FlightNumber, req.Status, ct);
        await _hub.Clients.Group(req.FlightNumber).SendAsync("FlightStatusUpdated", req.FlightNumber, req.Status, ct);
        return dto;
    }
}
