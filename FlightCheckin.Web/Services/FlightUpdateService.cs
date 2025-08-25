using Microsoft.AspNetCore.SignalR.Client;
using FlightCheckin.Models;

namespace FlightCheckin.Web.Services;

public interface IFlightUpdateService
{
    event Action<string, FlightStatus>? FlightStatusUpdated;
    event Action<string, string>? SeatAssigned;
    event Action<string>? SeatsUpdated;
    Task StartAsync();
    Task JoinFlightGroup(string flightNumber);
    Task StopAsync();
}

public class FlightUpdateService : IFlightUpdateService, IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FlightUpdateService> _logger;

    public event Action<string, FlightStatus>? FlightStatusUpdated;
    public event Action<string, string>? SeatAssigned;
    public event Action<string>? SeatsUpdated;

    public FlightUpdateService(IConfiguration configuration, ILogger<FlightUpdateService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        if (_hubConnection != null)
            return;

        var serverBaseUrl = _configuration["ServerBaseUrl"] ?? "http://localhost:5051/";
        
        // Ensure the base URL ends with a slash for proper URL concatenation
        if (!serverBaseUrl.EndsWith("/"))
            serverBaseUrl += "/";
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{serverBaseUrl}flightStatusHub")
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, FlightStatus>("FlightStatusUpdated", (flightNumber, status) =>
        {
            _logger.LogInformation($"Flight status updated: {flightNumber} -> {status}");
            FlightStatusUpdated?.Invoke(flightNumber, status);
        });

        _hubConnection.On<string, string>("SeatAssigned", (flightNumber, seatCode) =>
        {
            _logger.LogInformation($"Seat assigned: {flightNumber} -> {seatCode}");
            SeatAssigned?.Invoke(flightNumber, seatCode);
        });

        _hubConnection.On<string>("SeatsUpdated", (flightNumber) =>
        {
            _logger.LogInformation($"Seats updated for flight: {flightNumber}");
            SeatsUpdated?.Invoke(flightNumber);
        });

        try
        {
            await _hubConnection.StartAsync();
            _logger.LogInformation("SignalR connection established successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignalR connection error");
        }
    }

    public async Task JoinFlightGroup(string flightNumber)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync("JoinFlightGroup", flightNumber);
                _logger.LogInformation($"Joined group for flight: {flightNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error joining flight group {flightNumber}");
            }
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
