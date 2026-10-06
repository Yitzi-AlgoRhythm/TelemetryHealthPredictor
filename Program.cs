using TelemetryHealthPredictor.Services.Connection;
using TelemetryHealthPredictor.Setup;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.RegisterServices();

WebApplication app = builder.Build();

app.MapHub<UAVHealthDataHub>("/uav-health-data");

app.UseHttpsRedirection();

app.Run();