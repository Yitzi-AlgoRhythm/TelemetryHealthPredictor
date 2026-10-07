using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations
{
    public class KalmanFilter : IKalmanFilter
    {
        private readonly KalmanData _kalmanData;

        public KalmanFilter(KalmanData kalmanData)
        {
            _kalmanData = kalmanData;
        }

        public Vector<double>[] KalmanFilterDelegate(KeyValuePair<string, double> parameterValue)
        {
            if (_kalmanData.AllMeasurementsIn(parameterValue))
            {
                return [_kalmanData.RunKalmanFilter()]; 
            }

            return [];
        }
    }
}
