using FlightCheckin.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient("server", c =>
{
    // Server API өөр port дээр байж болно. Development үед нэг шийдэлд хамтад нь ажиллуулбал http://localhost:5000 гэж тааруул.
    c.BaseAddress = new Uri("http://localhost:5051/");
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

