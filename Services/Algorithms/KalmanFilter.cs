using TelemetryHealthPredictor.Config;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms
{
    public class KalmanFilter
    {
        private readonly KalmanSetupData _setupData;

        public KalmanFilter(KalmanSetupData setupData)
        {
            _setupData = setupData;
        }

        public FilteredResult KalmanFilterDelegate(KeyValuePair<string, double> parameterValue)
        {

        }
    }
}
