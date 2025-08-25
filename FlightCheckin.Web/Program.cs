using FlightCheckin.Web.Components;
using FlightCheckin.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient("server", c =>
{
    // Server API өөр port дээр байж болно. Development үед нэг шийдэлд хамтад нь ажиллуулбал http://localhost:5000 гэж тааруул.
    c.BaseAddress = new Uri("http://localhost:5051/");
});

// Register the SignalR service
builder.Services.AddSingleton<IFlightUpdateService, FlightUpdateService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery(); // Anti-forgery middleware нэмсэн
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Start the SignalR service with error handling
try
{
    var flightUpdateService = app.Services.GetRequiredService<IFlightUpdateService>();
    await flightUpdateService.StartAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Failed to start SignalR service. The application will continue without real-time updates.");
}

app.Run();

