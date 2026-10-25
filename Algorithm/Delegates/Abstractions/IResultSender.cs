using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Abstractions
{
    public interface IResultSender
    {
        public Task ResultSenderDelegate(AlgorithmResult result);
    }
}
