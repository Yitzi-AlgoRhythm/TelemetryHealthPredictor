using Microsoft.AspNetCore.SignalR;
using TelemetryHealthPredictor.Algorithm.Delegates.Abstractions;
using TelemetryHealthPredictor.Data;
using TelemetryHealthPredictor.Setup.Connection;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Implementations
{
    public class ResultSender : IResultSender
    {
        private readonly IHubContext<UAVHealthDataHub> _hubContext;

        public ResultSender(IHubContext<UAVHealthDataHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task ResultSenderDelegate(AlgorithmResult result)
        {
            await _hubContext.Clients.All.SendAsync("UAVHealthDataUpdate", result);
        }
    }
}