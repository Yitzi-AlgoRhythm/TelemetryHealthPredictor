using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface IResultSender
    {
        public Task ResultSenderDelegate(AlgorithmResult result);
    }
}
