using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using FlightCheckin.BusinessLogic.Services;
using FlightCheckin.Models;

namespace FlightCheckin.Server.Services;

/// <summary>
/// Энгийн TCP сокет сервер: JSON мөрөөр Check-in хүсэлтийг авна.
/// { "FlightNumber":"MGL101", "PassportNumber":"A1234567", "SeatRow":1, "SeatColumn":"A" }
/// </summary>
public class SocketServerService : BackgroundService
{
    private readonly ILogger<SocketServerService> _logger;
    private readonly IServiceProvider _services;

    public SocketServerService(ILogger<SocketServerService> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = new TcpListener(IPAddress.Any, 8888);
        listener.Start();
        _logger.LogInformation("Socket server started on 8888");
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken);
                _ = HandleClientAsync(client, stoppingToken);
            }
        }
        catch (OperationCanceledException) { }
        finally { listener.Stop(); }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var checkin = scope.ServiceProvider.GetRequiredService<ICheckinService>();

        await using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        await using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        try
        {
            var line = await reader.ReadLineAsync(ct);
            if (line is null) return;

            var req = JsonSerializer.Deserialize<CheckinRequest>(line,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                      ?? throw new InvalidOperationException("Invalid request");

            var res = await checkin.AssignSeatAsync(req, ct);
            var json = JsonSerializer.Serialize(res);
            await writer.WriteLineAsync(json);
        }
        catch (Exception ex)
        {
            var json = JsonSerializer.Serialize(new CheckinResponse(false, ex.Message, null, null));
            await writer.WriteLineAsync(json);
        }
        finally
        {
            client.Close();
        }
    }
}
