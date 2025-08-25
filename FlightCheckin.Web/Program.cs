using FlightCheckin.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient("server", c =>
{
    // Use the new server port
    c.BaseAddress = new Uri("http://localhost:5002/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery(); // Anti-forgery middleware нэмсэн
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

