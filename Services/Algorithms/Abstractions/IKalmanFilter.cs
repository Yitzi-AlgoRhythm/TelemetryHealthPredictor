using TelemetryHealthPredictor.Config;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface IKalmanFilter
    {
        public FilteredResult KalmanFilterDelegate(KeyValuePair<string, double> parameterValue);
    }
}
