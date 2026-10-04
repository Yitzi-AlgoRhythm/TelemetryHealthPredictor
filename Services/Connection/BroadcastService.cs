using Microsoft.AspNetCore.SignalR;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Connection
{
    public class BroadcastService
    {
        private readonly IHubContext<UAVHealthDataHub> _hubContext;

        public BroadcastService(IHubContext<UAVHealthDataHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastDelegate(AlgorithmResult result)
        {
            await _hubContext.Clients.All.SendAsync("UAVHealthDataUpdate", result);
        }
    }
}
