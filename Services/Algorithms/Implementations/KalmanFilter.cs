using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
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

        public KalmanFilterResult[] KalmanFilterDelegate(IncomingFlightParameter[] parameters)
        {
            //if (_kalmanData.AllMeasurementsIn(parameter))
            //{
            //    return [_kalmanData.RunKalmanFilter()]; 
            //}

            return [];
        }
    }
}
