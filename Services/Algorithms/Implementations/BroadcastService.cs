using Microsoft.AspNetCore.SignalR;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;
using TelemetryHealthPredictor.Services.Connection;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations
{
    public class BroadcastService : IResultSender
    {
        private readonly IHubContext<UAVHealthDataHub> _hubContext;

        public BroadcastService(IHubContext<UAVHealthDataHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task ResultSenderDelegate(AlgorithmResult result)
        {
            await _hubContext.Clients.All.SendAsync("UAVHealthDataUpdate", result);
        }
    }
}