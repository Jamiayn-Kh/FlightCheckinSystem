using FlightCheckin.BusinessLogic.Services;
using FlightCheckin.DataAccess.Context;
using FlightCheckin.DataAccess.Repositories;
using FlightCheckin.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using FlightCheckin.Server.Hubs;
using FlightCheckin.Server.Services;
using FlightCheckin.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

// SQLite
builder.Services.AddDbContext<FlightContext>(opt =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "flights.db");
    opt.UseSqlite($"Data Source={path}");
});

// DI
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<ICheckinService, CheckinService>();
builder.Services.AddScoped<IFlightService, FlightService>();

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:7075", "http://localhost:5002")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// Socket server (TCP 8888)
builder.Services.AddHostedService<SocketServerService>();

var app = builder.Build();

// Database seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FlightContext>();
    context.Database.EnsureCreated();
    
    // Seed initial data if database is empty
    if (!context.Flights.Any())
    {
        var flight = new Flight
        {
            FlightNumber = "MGL101",
            Destination = "Ulaanbaatar",
            DepartureTime = DateTime.Now.AddHours(2),
            Status = FlightStatus.CheckingIn
        };
        context.Flights.Add(flight);
        
        // Add some seats
        for (int row = 1; row <= 10; row++)
        {
            for (char col = 'A'; col <= 'F'; col++)
            {
                var seat = new Seat
                {
                    Row = row,
                    Column = col.ToString(),
                    IsTaken = false,
                    Flight = flight
                };
                context.Seats.Add(seat);
            }
        }
        
        context.SaveChanges();
    }
}

app.UseMiddleware<ErrorHandlingMiddleware>();

// Enable CORS
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.MapHub<FlightStatusHub>("/flightStatusHub");

app.Run();
